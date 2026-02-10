using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CreationEditor;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Services.Record.Prefix;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.State;
using DynamicData;
using DynamicData.Binding;
using FluentAvalonia.UI.Controls;
using LeveledList.Model;
using LeveledList.Model.List;
using LeveledList.Services;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace LeveledList.ViewModels;

public sealed partial class ListsVM : ValidatableViewModel {
    private readonly ILogger _logger;
    private readonly ITierController _tierController;
    private readonly LeveledListGenerator _generator;
    private readonly LeveledListImplementer _implementer;
    private readonly IRecordPrefixService _recordPrefixService;

    public GenerationConfigurationVM GenerationConfig { get; }

    private readonly ObservableCollectionExtended<LeveledListTreeNode> _leveledLists = [];
    public ReadOnlyObservableCollection<LeveledListTreeNode> LeveledLists { get; }
    public IObservableCollection<ExtendedListDefinition> ListTypeDefinitions { get; } = new ObservableCollectionExtended<ExtendedListDefinition>();

    [Reactive] public partial IReadOnlyList<ExtendedListDefinition>? SelectedDefinitions { get; set; }

    public HierarchicalTreeDataGridSource<LeveledListTreeNode> LeveledListSource { get; }
    public ReactiveCommand<Unit, Unit> GenerateSelectedLists { get; }
    public ReactiveCommand<Unit, Unit> ReloadDefinitions { get; }

    public ListsVM(
        IStateRepositoryFactory<LeveledListMemento, LeveledListMemento, Guid> stateRepositoryFactory,
        ILogger logger,
        ISearchFilter searchFilter,
        ITierController tierController,
        LeveledListGenerator generator,
        LeveledListImplementer implementer,
        GenerationConfigurationVM generationConfig,
        IRecordPrefixService recordPrefixService) {
        _logger = logger;
        _tierController = tierController;
        _generator = generator;
        _implementer = implementer;
        _recordPrefixService = recordPrefixService;

        GenerationConfig = generationConfig;

        var stateRepository = stateRepositoryFactory.Create("LeveledList");
        var leveledListMemento = stateRepository.LoadAllWithIdentifier().FirstOrDefault();
        Guid? leveledListMementoGuid = leveledListMemento.Value is null ? null : leveledListMemento.Key;
        GenerationConfig.DefinitionsFolderPath = leveledListMemento.Value?.LeveledListFolderPath;

        var filter = GenerationConfig.WhenAnyValue(x => x.FilterText)
            .NotNull()
            .Select<string, Func<LeveledListTreeNode, bool>>(filter =>
                node => searchFilter.Filter(node.LeveledList?.EditorID ?? string.Empty, filter))
            .StartWith(_ => true);

        LeveledLists = _leveledLists
            .ObserveCollectionChanges()
            .Select(_ => _leveledLists.AsObservableChangeSet())
            .Switch()
            .Filter(filter)
            .ToObservableCollection(this);

        GenerateSelectedLists = ReactiveCommand.CreateRunInBackground(GenerateLeveledLists);
        ReloadDefinitions = ReactiveCommand.Create(() => {
            if (GenerationConfig.DefinitionsFolderPath is null) return;

            LoadDefinitions(GenerationConfig.DefinitionsFolderPath);
        });

        LeveledListSource = new HierarchicalTreeDataGridSource<LeveledListTreeNode>(LeveledLists) {
            Columns = {
                new TemplateColumn<LeveledListTreeNode>(
                    "Exists",
                    new FuncDataTemplate<LeveledListTreeNode>((x, _) => new TextBlock {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = x?.ExistingLeveledItem is not null ? "✅" : string.Empty
                    }),
                    options: new TemplateColumnOptions<LeveledListTreeNode> {
                        CompareAscending = (x, y) => {
                            if (x is null && y is null) return 0;
                            if (x is null) return -1;
                            if (y is null) return 1;

                            var xExists = x.ExistingLeveledItem is not null;
                            var yExists = y.ExistingLeveledItem is not null;
                            return xExists.CompareTo(yExists);
                        },
                        CompareDescending = (x, y) => {
                            if (x is null && y is null) return 0;
                            if (x is null) return 1;
                            if (y is null) return -1;

                            var xExists = x.ExistingLeveledItem is not null;
                            var yExists = y.ExistingLeveledItem is not null;
                            return yExists.CompareTo(xExists);
                        }
                    }
                ),
                new HierarchicalExpanderColumn<LeveledListTreeNode>(
                    new TemplateColumn<LeveledListTreeNode>(
                        "EditorID",
                        new FuncDataTemplate<LeveledListTreeNode>((r, _) => new TextBlock {
                            VerticalAlignment = VerticalAlignment.Center,
                            Text = r?.LeveledList?.EditorID ?? r?.Entry?.Item.EditorID
                        }),
                        options: new TemplateColumnOptions<LeveledListTreeNode> {
                            CompareAscending = (x, y) =>
                                x?.LeveledList?.EditorID.CompareTo(y?.LeveledList?.EditorID, StringComparison.OrdinalIgnoreCase)
                             ?? x?.Entry?.Item.EditorID.CompareTo(y?.Entry?.Item.EditorID, StringComparison.OrdinalIgnoreCase)
                             ?? 0,
                            CompareDescending = (x, y) =>
                                y?.LeveledList?.EditorID.CompareTo(x?.LeveledList?.EditorID, StringComparison.OrdinalIgnoreCase)
                             ?? y?.Entry?.Item.EditorID.CompareTo(x?.Entry?.Item.EditorID, StringComparison.OrdinalIgnoreCase)
                             ?? 0,
                        }
                    ),
                    item => SelectNodes(item.LeveledList)
                     ?? SelectNodes(item.Entry?.Item.List)
                ),
                new TemplateColumn<LeveledListTreeNode>(
                    "Level",
                    new FuncDataTemplate<LeveledListTreeNode>((x, _) => new TextBlock {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = x?.Entry?.Level.ToString()
                    }),
                    options: new TemplateColumnOptions<LeveledListTreeNode> {
                        CompareAscending = (x, y) => x?.Entry?.Level.CompareTo(y?.Entry?.Level) ?? 0,
                        CompareDescending = (x, y) => y?.Entry?.Level.CompareTo(x?.Entry?.Level) ?? 0,
                    }),
                new TemplateColumn<LeveledListTreeNode>(
                    "Count",
                    new FuncDataTemplate<LeveledListTreeNode>((x, _) => new TextBlock {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = x?.Entry?.Count.ToString()
                    }),
                    options: new TemplateColumnOptions<LeveledListTreeNode> {
                        CompareAscending = (x, y) => x?.Entry?.Count.CompareTo(y?.Entry?.Count) ?? 0,
                        CompareDescending = (x, y) => y?.Entry?.Count.CompareTo(x?.Entry?.Count) ?? 0,
                    }),
            }
        };

        GenerationConfig.WhenAnyValue(x => x.DefinitionsFolderPath)
            .NotNull()
            .Subscribe(path => {
                LoadDefinitions(path);
                stateRepository.Update(
                    memento => memento is null
                        ? new LeveledListMemento(path, string.Empty)
                        : memento with { LeveledListFolderPath = path },
                    leveledListMementoGuid ??= Guid.NewGuid());
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedDefinitions)
            .CombineLatest(
                GenerationConfig.ModPicker.SelectedMods,
                GenerationConfig.RecordPrefixContent.RecordPrefixService.PrefixChanged.ThrottleMedium(),
                GenerationConfig.EditorIdReplacement.WhenAnyValue(x => x.Pattern, x => x.Replacement).ThrottleMedium(),
                (def, mods, _, _) => (Definitions: def, SelectedMods: mods))
            .ThrottleShort()
            .ObserveOnTaskpool()
            .Subscribe(x => UpdateListsShowcase(x.Definitions, x.SelectedMods))
            .DisposeWith(this);
    }

    private void LoadDefinitions(string directoryPath) {
        ListTypeDefinitions.LoadOptimized(GetDefinitions(directoryPath));
    }

    private void GenerateLeveledLists() {
        if (SelectedDefinitions is null) return;

        var selectedModsItems = GenerationConfig.ModPicker.GetSelectedMods();
        if (selectedModsItems.Count == 0) return;

        var selectedMods = GenerationConfig.EditorEnvironment.ResolveMods(selectedModsItems.Select(x => x.ModKey)).ToArray();
        if (selectedMods.Length == 0) return;

        // Generate leveled list intermediate format based on type definition
        var generatedLists = new List<Model.List.LeveledList>();
        foreach (var listTypeDefinition in SelectedDefinitions.Select(x => x.TypeDefinition).Distinct()) {
            var tierAliases = _tierController.GetTierAliases(listTypeDefinition.Type);
            var leveledLists = _generator.Generate(listTypeDefinition, selectedMods, GenerationConfig.EditorIdReplacement.Replace);
            generatedLists.AddRange(leveledLists
                .Where(x => SelectedDefinitions.Any(list => list.Matches(x, tierAliases))));
        }

        _implementer.ImplementLeveledLists(GenerationConfig.EditorEnvironment.ActiveMod, generatedLists);
    }

    private IEnumerable<LeveledListTreeNode>? SelectNodes(Model.List.LeveledList? list) {
        return list?.Entries
            .Select(entry => new LeveledListTreeNode(
                null,
                entry.Item.EditorID is not null && GenerationConfig.EditorEnvironment.LinkCache.TryResolve<IItemGetter>(entry.Item.EditorID, out var existingLeveledItem)
                    ? existingLeveledItem
                    : null,
                entry));
    }

    private void UpdateListsShowcase(IReadOnlyList<ExtendedListDefinition>? selectedLists, IReadOnlyCollection<OrderedModItem> selectedMods) {
        var mods = GenerationConfig.EditorEnvironment.ResolveMods(selectedMods.Select(x => x.ModKey)).ToArray();
        if (mods.Length == 0 || selectedLists is null) {
            Dispatcher.UIThread.Post(() => _leveledLists.Clear());
            return;
        }

        try {
            GenerationConfig.SetBusy(true);
            // Generate leveled list intermediate format based on type definition
            var generatedLists = new List<LeveledListTreeNode>();
            foreach (var listTypeDefinition in selectedLists.Select(x => x.TypeDefinition).Distinct()) {
                var tierAliases = _tierController.GetTierAliases(listTypeDefinition.Type);
                var leveledLists = _generator.Generate(listTypeDefinition, mods, GenerationConfig.EditorIdReplacement.Replace);
                generatedLists.AddRange(leveledLists
                    .Where(x => selectedLists.Any(list => list.Matches(x, tierAliases)))
                    .Select(l => new LeveledListTreeNode(
                        l,
                        GenerationConfig.EditorEnvironment.LinkCache.TryResolve<ILeveledItemGetter>(l.EditorID, out var existingLeveledItem)
                            ? existingLeveledItem
                            : null,
                        null)));
            }

            Dispatcher.UIThread.Post(() => {
                _leveledLists.LoadOptimized(generatedLists);
                GenerationConfig.SetBusy(false);
            });
        } catch (Exception e) {
            Dispatcher.UIThread.Post(() => {
                _leveledLists.Clear();
                GenerationConfig.SetBusy(false);
            });
            _logger.Here().Error(e,
                "Failed to generate leveled lists for {ListDefinition}",
                string.Join(',', selectedLists.Select(x => x.ListDefinition.Name)));
        }
    }

    private IEnumerable<ExtendedListDefinition> GetDefinitions(string directoryPath) {
        return GenerationConfig.GetDefinitionsFromYaml<ExtendedListDefinition>(
            directoryPath,
            (deserializer, reader, file, fileName) => {
                var listTypeDefinition = deserializer.Deserialize<ListTypeDefinition>(reader);
                return listTypeDefinition.Lists.Select(kvp =>
                    new ExtendedListDefinition(file, fileName, listTypeDefinition, kvp.Key, kvp.Value, _recordPrefixService));
            });
    }

    public void ContextMenu(object? sender, ContextRequestedEventArgs e) {
        if (e.Source is not Control control) return;

        var treeDataGrid = control.FindAncestorOfType<TreeDataGrid>();
        if (treeDataGrid?.RowSelection is null) return;

        var records = treeDataGrid.RowSelection.SelectedItems
            .OfType<LeveledListTreeNode>()
            .Select(x => x.Entry?.Item.Record)
            .WhereNotNull()
            .ToList();

        if (records.Count > 0) {
            var contextFlyout = new MenuFlyout {
                Items = {
                    new MenuItem {
                        Header = "Remove from Tier",
                        Icon = new SymbolIcon { Symbol = Symbol.Remove },
                        Command = ReactiveCommand.Create(() => {
                            foreach (var record in records) {
                                _tierController.RemoveRecordTier(record);
                            }

                            UpdateListsShowcase(SelectedDefinitions, GenerationConfig.ModPicker.GetSelectedMods());
                        })
                    },
                },
            };

            contextFlyout.ShowAt(control, true);
        }
    }
}
