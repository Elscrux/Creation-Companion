using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Threading;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.State;
using DynamicData;
using DynamicData.Binding;
using LeveledList.Model.List;
using LeveledList.Resources;
using LeveledList.Services.LeveledList;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace LeveledList.ViewModels;

public sealed partial class ListsVM : ValidatableViewModel {
    private readonly IFileSystem _fileSystem;
    private readonly LeveledListGenerator _generator;

    private readonly IObservableCollection<LeveledListTreeNode> _leveledLists = new ObservableCollectionExtended<LeveledListTreeNode>();
    public ReadOnlyObservableCollection<LeveledListTreeNode> LeveledLists { get; }
    public IObservableCollection<ExtendedListDefinition> ListTypeDefinitions { get; } = new ObservableCollectionExtended<ExtendedListDefinition>();

    [Reactive] public partial string? LeveledListFolderPath { get; set; }
    [Reactive] public partial ExtendedListDefinition? SelectedList { get; set; } = null!;
    [Reactive] public partial string? LeveledListFilter { get; set; }

    public HierarchicalTreeDataGridSource<LeveledListTreeNode> LeveledListSource { get; }
    public SingleModPickerVM ModPickerVM { get; }

    public ListsVM(
        IStateRepositoryFactory<LeveledListMemento, LeveledListMemento, Guid> stateRepositoryFactory,
        SingleModPickerVM modPickerVM,
        ILogger logger,
        IFileSystem fileSystem,
        IEditorEnvironment editorEnvironment,
        ISearchFilter searchFilter,
        LeveledListGenerator generator) {
        ModPickerVM = modPickerVM;
        _fileSystem = fileSystem;
        _generator = generator;

        var stateRepository = stateRepositoryFactory.Create("LeveledList");
        var leveledListMemento = stateRepository.LoadAllWithIdentifier().FirstOrDefault();
        LeveledListFolderPath = leveledListMemento.Value?.LeveledListFolderPath;

        var leveledListObservable = this.WhenAnyValue(x => x.LeveledListFilter)
            .NotNull()
            .Select<string, Func<LeveledListTreeNode, bool>>(filter =>
                node => searchFilter.Filter(node.LeveledList?.EditorID ?? string.Empty, filter))
            .StartWith(_ => true);

        LeveledLists = _leveledLists
            .ObserveCollectionChanges()
            .Select(_ => _leveledLists.AsObservableChangeSet())
            .Switch()
            .Filter(leveledListObservable)
            .ToObservableCollection(this);

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
                ListTypeDefinitions.Load(GetDefinitions(path));
                stateRepository.Save(
                    new LeveledListMemento(path),
                    leveledListMemento.Value is null
                        ? Guid.NewGuid()
                        : leveledListMemento.Key);
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedList)
            .CombineLatest(ModPickerVM.SelectedModChanged, (def, mod) => (Definition: def, SelectedMod: mod))
            .ObserveOnTaskpool()
            .Subscribe(x => {
                var mod = editorEnvironment.ResolveMod(x.SelectedMod?.ModKey);
                if (mod is null || x.Definition is null) {
                    Dispatcher.UIThread.Post(() => _leveledLists.Clear());
                    return;
                }

                try {
                    var lists = _generator.Generate(x.Definition.TypeDefinition, mod)
                        .Where(list => {
                            var featureWildcards = x.Definition.ListDefinition.Name.GetFeatureWildcards().ToArray();
                            if (list.Features.Count != featureWildcards.Length) return false;
                            if (list.Features.Any(f => !featureWildcards.Contains(f.Wildcard.Identifier))) return false;

                            return list.EditorID == x.Definition.ListDefinition.GetFullName(list.Features);
                        })
                        .Select(l => new LeveledListTreeNode(l, null))
                        .ToArray();
                    Dispatcher.UIThread.Post(() => _leveledLists.Load(lists));
                } catch (Exception e) {
                    Dispatcher.UIThread.Post(() => _leveledLists.Clear());
                    logger.Here().Error(e, "Failed to generate leveled lists for {ListDefinition}", x.Definition.ListDefinition.Name);

                }
            })
            .DisposeWith(this);
    }

    private static IEnumerable<LeveledListTreeNode>? SelectNodes(Model.List.LeveledList? list) {
        return list?.Entries
            .Select(entry => new LeveledListTreeNode(null, entry));
    }

    private IEnumerable<ExtendedListDefinition> GetDefinitions(string directoryPath) {
        if (!_fileSystem.Directory.Exists(directoryPath)) yield break;

        var deserializer = new DeserializerBuilder()
            .WithTypeConverter(new FormKeyYamlTypeConverter())
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();

        foreach (var file in _fileSystem.Directory.EnumerateFiles(directoryPath, "*.yaml", SearchOption.AllDirectories)) {
            var fileStream = _fileSystem.File.Open(file, FileMode.Open);

            ListTypeDefinition listTypeDefinition;
            try {
                listTypeDefinition = deserializer.Deserialize<ListTypeDefinition>(new StreamReader(fileStream));
            } catch (Exception) {
                continue;
            }

            foreach (var (listName, list) in listTypeDefinition.Lists) {
                yield return new ExtendedListDefinition(listTypeDefinition, listName, list);
            }
        }
    }
}
