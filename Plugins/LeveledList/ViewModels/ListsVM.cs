using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Record.Prefix;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.State;
using DynamicData;
using DynamicData.Binding;
using FluentAvalonia.UI.Controls;
using LeveledList.Model;
using LeveledList.Model.List;
using LeveledList.Resources;
using LeveledList.Services;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace LeveledList.ViewModels;

public sealed partial class ListsVM : ValidatableViewModel {
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly ITierController _tierController;
    private readonly LeveledListGenerator _generator;
    private readonly LeveledListImplementer _implementer;
    private readonly IRecordPrefixService _recordPrefixService;

    private readonly ObservableCollectionExtended<LeveledListTreeNode> _leveledLists = new();
    public ReadOnlyObservableCollection<LeveledListTreeNode> LeveledLists { get; }
    public IObservableCollection<ExtendedListDefinition> ListTypeDefinitions { get; } = new ObservableCollectionExtended<ExtendedListDefinition>();

    [Reactive] public partial string? LeveledListFolderPath { get; set; }
    [Reactive] public partial IReadOnlyList<ExtendedListDefinition>? SelectedLists { get; set; } = null;
    [Reactive] public partial string? LeveledListFilter { get; set; }

    public HierarchicalTreeDataGridSource<LeveledListTreeNode> LeveledListSource { get; }
    public MultiModPickerVM ModPickerVM { get; }
    public RecordPrefixVM RecordPrefixVM { get; }
    public ReactiveCommand<Unit, Unit> GenerateSelectedLists { get; }
    public ReactiveCommand<Unit, Unit> ReloadLists { get; }

    private readonly Subject<bool> _isBusy = new();
    public IObservable<bool> IsBusy => _isBusy;

    public ListsVM(
        IStateRepositoryFactory<LeveledListMemento, LeveledListMemento, Guid> stateRepositoryFactory,
        MultiModPickerVM modPickerVM,
        RecordPrefixVM recordPrefixVM,
        ILogger logger,
        IFileSystem fileSystem,
        IEditorEnvironment editorEnvironment,
        ISearchFilter searchFilter,
        ITierController tierController,
        LeveledListGenerator generator,
        LeveledListImplementer implementer,
        IRecordPrefixService recordPrefixService) {
        ModPickerVM = modPickerVM;
        RecordPrefixVM = recordPrefixVM;
        _logger = logger;
        _fileSystem = fileSystem;
        _editorEnvironment = editorEnvironment;
        _tierController = tierController;
        _generator = generator;
        _implementer = implementer;
        _recordPrefixService = recordPrefixService;

        var stateRepository = stateRepositoryFactory.Create("LeveledList");
        var leveledListMemento = stateRepository.LoadAllWithIdentifier().FirstOrDefault();
        Guid? leveledListMementoGuid = leveledListMemento.Value is null ? null : leveledListMemento.Key;
        LeveledListFolderPath = leveledListMemento.Value?.LeveledListFolderPath;

        var filter = this.WhenAnyValue(x => x.LeveledListFilter)
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
        ReloadLists = ReactiveCommand.Create(() => {
            if (LeveledListFolderPath is null) return;

            ListTypeDefinitions.LoadOptimized(GetDefinitions(LeveledListFolderPath));
        });

        LeveledListSource = new HierarchicalTreeDataGridSource<LeveledListTreeNode>(LeveledLists) {
            Columns = {
                new TemplateColumn<LeveledListTreeNode>(
                    "Existing",
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

        this.WhenAnyValue(x => x.LeveledListFolderPath)
            .NotNull()
            .Subscribe(path => {
                ListTypeDefinitions.LoadOptimized(GetDefinitions(path));
                stateRepository.Update(
                    memento => memento is null
                        ? new LeveledListMemento(path, string.Empty)
                        : memento with { LeveledListFolderPath = path },
                    leveledListMementoGuid ??= Guid.NewGuid());
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedLists)
            .CombineLatest(ModPickerVM.SelectedMods, (def, mods) => (Definitions: def, SelectedMods: mods))
            .ThrottleShort()
            .ObserveOnTaskpool()
            .Subscribe(x => UpdateListsShowcase(x.Definitions, x.SelectedMods))
            .DisposeWith(this);
    }

    private void GenerateLeveledLists() {
        if (SelectedLists is null) return;

        var selectedModsItems = ModPickerVM.GetSelectedMods();
        if (selectedModsItems.Count == 0) return;

        var selectedMods = _editorEnvironment.ResolveMods(selectedModsItems.Select(x => x.ModKey)).ToArray();
        if (selectedMods.Length == 0) return;

        // Generate leveled list intermediate format based on type definition
        var generatedLists = new List<Model.List.LeveledList>();
        foreach (var listTypeDefinition in SelectedLists.Select(x => x.TypeDefinition).Distinct()) {
            var tierAliases = _tierController.GetTierAliases(listTypeDefinition.Type);
            var leveledLists = _generator.Generate(listTypeDefinition, selectedMods);
            generatedLists.AddRange(leveledLists
                .Where(x => SelectedLists.Any(list => list.Matches(x, tierAliases))));
        }

        _implementer.ImplementLeveledLists(_editorEnvironment.ActiveMod, generatedLists);
    }

    private IEnumerable<LeveledListTreeNode>? SelectNodes(Model.List.LeveledList? list) {
        return list?.Entries
            .Select(entry => new LeveledListTreeNode(
                null,
                entry.Item.EditorID is not null && _editorEnvironment.LinkCache.TryResolve<IItemGetter>(entry.Item.EditorID, out var existingLeveledItem)
                    ? existingLeveledItem
                    : null,
                entry));
    }

    private void UpdateListsShowcase(IReadOnlyList<ExtendedListDefinition>? selectedLists, IReadOnlyCollection<OrderedModItem> selectedMods) {
        var mods = _editorEnvironment.ResolveMods(selectedMods.Select(x => x.ModKey)).ToArray();
        if (mods.Length == 0 || selectedLists is null) {
            Dispatcher.UIThread.Post(() => _leveledLists.Clear());
            return;
        }

        try {
            _isBusy.OnNext(true);
            // Generate leveled list intermediate format based on type definition
            var generatedLists = new List<LeveledListTreeNode>();
            foreach (var listTypeDefinition in selectedLists.Select(x => x.TypeDefinition).Distinct()) {
                var tierAliases = _tierController.GetTierAliases(listTypeDefinition.Type);
                var leveledLists = _generator.Generate(listTypeDefinition, mods);
                generatedLists.AddRange(leveledLists
                    .Where(x => selectedLists.Any(list => list.Matches(x, tierAliases)))
                    .Select(l => new LeveledListTreeNode(
                        l,
                        _editorEnvironment.LinkCache.TryResolve<ILeveledItemGetter>(l.EditorID, out var existingLeveledItem)
                            ? existingLeveledItem
                            : null,
                        null)));
            }

            Dispatcher.UIThread.Post(() => {
                _leveledLists.LoadOptimized(generatedLists);
                _isBusy.OnNext(false);
            });
        } catch (Exception e) {
            Dispatcher.UIThread.Post(() => {
                _leveledLists.Clear();
                _isBusy.OnNext(false);
            });
            _logger.Here().Error(e,
                "Failed to generate leveled lists for {ListDefinition}",
                string.Join(',', selectedLists.Select(x => x.ListDefinition.Name)));
        }
    }

    private IEnumerable<ExtendedListDefinition> GetDefinitions(string directoryPath) {
        if (!_fileSystem.Directory.Exists(directoryPath)) yield break;

        var deserializer = new DeserializerBuilder()
            .WithTypeConverter(new FormKeyYamlTypeConverter())
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();

        foreach (var file in _fileSystem.Directory.EnumerateFiles(directoryPath, "*.yaml", SearchOption.AllDirectories)) {
            using var fileStream = _fileSystem.File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = _fileSystem.Path.GetFileNameWithoutExtension(file);

            ListTypeDefinition listTypeDefinition;
            try {
                listTypeDefinition = deserializer.Deserialize<ListTypeDefinition>(new StreamReader(fileStream));
            } catch (Exception) {
                continue;
            }

            foreach (var (listName, list) in listTypeDefinition.Lists) {
                yield return new ExtendedListDefinition(file, fileName, listTypeDefinition, listName, list, _recordPrefixService);
            }
        }
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

                            UpdateListsShowcase(SelectedLists, ModPickerVM.GetSelectedMods());
                        })
                    },
                },
            };

            contextFlyout.ShowAt(control, true);
        }
    }
}
