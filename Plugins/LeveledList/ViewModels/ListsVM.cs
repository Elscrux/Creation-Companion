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
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
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

    private readonly ObservableCollectionExtended<LeveledListTreeNode> _leveledLists = new();
    public ReadOnlyObservableCollection<LeveledListTreeNode> LeveledLists { get; }
    public IObservableCollection<ExtendedListDefinition> ListTypeDefinitions { get; } = new ObservableCollectionExtended<ExtendedListDefinition>();

    [Reactive] public partial string? LeveledListFolderPath { get; set; }
    [Reactive] public partial IReadOnlyList<ExtendedListDefinition>? SelectedLists { get; set; } = null;
    [Reactive] public partial string? LeveledListFilter { get; set; }

    public HierarchicalTreeDataGridSource<LeveledListTreeNode> LeveledListSource { get; }
    public SingleModPickerVM ModPickerVM { get; }
    public ReactiveCommand<Unit, Unit> GenerateSelectedLists { get; }
    public ReactiveCommand<Unit, Unit> ReloadLists { get; }

    private readonly Subject<bool> _isBusy = new();
    public IObservable<bool> IsBusy => _isBusy;

    public ListsVM(
        IStateRepositoryFactory<LeveledListMemento, LeveledListMemento, Guid> stateRepositoryFactory,
        SingleModPickerVM modPickerVM,
        ILogger logger,
        IFileSystem fileSystem,
        IEditorEnvironment editorEnvironment,
        ISearchFilter searchFilter,
        ITierController tierController,
        LeveledListGenerator generator,
        LeveledListImplementer implementer) {
        ModPickerVM = modPickerVM;
        _logger = logger;
        _fileSystem = fileSystem;
        _editorEnvironment = editorEnvironment;
        _tierController = tierController;
        _generator = generator;
        _implementer = implementer;

        var stateRepository = stateRepositoryFactory.Create("LeveledList");
        var leveledListMemento = stateRepository.LoadAllWithIdentifier().FirstOrDefault();
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
                new HierarchicalExpanderColumn<LeveledListTreeNode>(
                    new TemplateColumn<LeveledListTreeNode>(
                        "EditorID",
                        new FuncDataTemplate<LeveledListTreeNode>((r, _) => new TextBlock {
                            VerticalAlignment = VerticalAlignment.Center,
                            Text = r?.LeveledList?.EditorID ?? r?.Entry?.Item.EditorID
                        })
                    ),
                    item => SelectNodes(item.LeveledList)
                     ?? SelectNodes(item.Entry?.Item.List)
                ),
                new TemplateColumn<LeveledListTreeNode>(
                    "Level",
                    new FuncDataTemplate<LeveledListTreeNode>((x, _) => new TextBlock {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = x?.Entry?.Level.ToString()
                    })),
                new TemplateColumn<LeveledListTreeNode>(
                    "Count",
                    new FuncDataTemplate<LeveledListTreeNode>((x, _) => new TextBlock {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = x?.Entry?.Count.ToString()
                    })),
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
                    leveledListMemento.Value is null
                        ? Guid.NewGuid()
                        : leveledListMemento.Key);
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedLists)
            .CombineLatest(ModPickerVM.SelectedModChanged, (def, mod) => (Definitions: def, SelectedMod: mod))
            .ThrottleShort()
            .ObserveOnTaskpool()
            .Subscribe(x => UpdateListsShowcase(x.Definitions, x.SelectedMod))
            .DisposeWith(this);
    }

    private void GenerateLeveledLists() {
        if (SelectedLists is null) return;
        if (ModPickerVM.SelectedMod is null) return;

        var selectedMod = _editorEnvironment.ResolveMod(ModPickerVM.SelectedMod.ModKey);
        if (selectedMod is null) return;

        // Generate leveled list intermediate format based on type definition
        var generatedLists = new List<Model.List.LeveledList>();
        foreach (var listTypeDefinition in SelectedLists.Select(x => x.TypeDefinition).Distinct()) {
            var leveledLists = _generator.Generate(listTypeDefinition, selectedMod);
            generatedLists.AddRange(leveledLists);
        }

        // Filter leveled lists based on the selected list definitions
        var leveledListToGenerate = generatedLists
            .Where(x => SelectedLists.Any(list => list.Matches(x)))
            .ToArray();

        _implementer.ImplementLeveledLists(_editorEnvironment.ActiveMod, leveledListToGenerate);
    }

    private static IEnumerable<LeveledListTreeNode>? SelectNodes(Model.List.LeveledList? list) {
        return list?.Entries
            .Select(entry => new LeveledListTreeNode(null, entry));
    }

    private void UpdateListsShowcase(IReadOnlyList<ExtendedListDefinition>? selectedLists, OrderedModItem? selectedMod) {
        var mod = _editorEnvironment.ResolveMod(selectedMod?.ModKey);
        if (mod is null || selectedLists is null) {
            Dispatcher.UIThread.Post(() => _leveledLists.Clear());
            return;
        }

        try {
            _isBusy.OnNext(true);
            // Generate leveled list intermediate format based on type definition
            var generatedLists = new List<Model.List.LeveledList>();
            foreach (var listTypeDefinition in selectedLists.Select(x => x.TypeDefinition).Distinct()) {
                var leveledLists = _generator.Generate(listTypeDefinition, mod);
                generatedLists.AddRange(leveledLists);
            }

            // Filter leveled lists based on the selected list definitions
            var lists = generatedLists
                .Where(x => selectedLists.Any(list => list.Matches(x)))
                .Select(l => new LeveledListTreeNode(l, null))
                .ToArray();

            Dispatcher.UIThread.Post(() => {
                _leveledLists.LoadOptimized(lists);
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
                yield return new ExtendedListDefinition(file, fileName, listTypeDefinition, listName, list);
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

                            UpdateListsShowcase(SelectedLists, ModPickerVM.SelectedMod);
                        })
                    },
                },
            };

            contextFlyout.ShowAt(control, true);
        }
    }
}
