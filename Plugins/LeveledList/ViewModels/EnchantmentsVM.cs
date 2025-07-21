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
using CreationEditor;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.State;
using DynamicData;
using DynamicData.Binding;
using LeveledList.Model;
using LeveledList.Model.Enchantments;
using LeveledList.Resources;
using LeveledList.Services.Enchantments;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace LeveledList.ViewModels;

public sealed partial class EnchantmentsVM : ViewModel {
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly EnchantmentsGenerator _generator;
    private readonly EnchantmentsImplementer _implementer;

    private readonly ObservableCollectionExtended<EnchantedItem> _enchantedItems = new();
    public ReadOnlyObservableCollection<EnchantedItem> EnchantedItems { get; }
    public IObservableCollection<ExtendedEnchantmentItem> EnchantmentsDefinitions { get; } = new ObservableCollectionExtended<ExtendedEnchantmentItem>();

    [Reactive] public partial string? DefinitionsFolderPath { get; set; }
    [Reactive] public partial IReadOnlyList<ExtendedEnchantmentItem>? SelectedEnchantmentItems { get; set; } = null;
    [Reactive] public partial string? EnchantedItemsFilter { get; set; }

    public FlatTreeDataGridSource<EnchantedItem> EnchantmentsSource { get; }
    public SingleModPickerVM ModPickerVM { get; }
    public ReactiveCommand<Unit, Unit> GenerateEnchantments { get; }
    public ReactiveCommand<Unit, Unit> ReloadLists { get; }

    private readonly Subject<bool> _isBusy = new();
    public IObservable<bool> IsBusy => _isBusy;

    public EnchantmentsVM(
        IStateRepositoryFactory<LeveledListMemento, LeveledListMemento, Guid> stateRepositoryFactory,
        SingleModPickerVM modPickerVM,
        ILogger logger,
        IFileSystem fileSystem,
        IEditorEnvironment editorEnvironment,
        ISearchFilter searchFilter,
        EnchantmentsGenerator generator,
        EnchantmentsImplementer implementer) {
        ModPickerVM = modPickerVM;
        _logger = logger;
        _fileSystem = fileSystem;
        _editorEnvironment = editorEnvironment;
        _generator = generator;
        _implementer = implementer;

        var stateRepository = stateRepositoryFactory.Create("Enchantments");
        var enchantmentsMemento = stateRepository.LoadAllWithIdentifier().FirstOrDefault();
        DefinitionsFolderPath = enchantmentsMemento.Value?.EnchantmentsFolderPath;

        var filter = this.WhenAnyValue(x => x.EnchantedItemsFilter)
            .NotNull()
            .Select<string, Func<EnchantedItem, bool>>(filter =>
                node => searchFilter.Filter(node.EditorID, filter))
            .StartWith(_ => true);

        EnchantedItems = _enchantedItems
            .ObserveCollectionChanges()
            .Select(_ => _enchantedItems.AsObservableChangeSet())
            .Switch()
            .Filter(filter)
            .ToObservableCollection(this);

        GenerateEnchantments = ReactiveCommand.CreateRunInBackground(GenerateEnchantedItems);
        ReloadLists = ReactiveCommand.Create(() => {
            if (DefinitionsFolderPath is null) return;

            EnchantmentsDefinitions.LoadOptimized(GetDefinitions(DefinitionsFolderPath));
        });

        EnchantmentsSource = new FlatTreeDataGridSource<EnchantedItem>(EnchantedItems) {
            Columns = {
                new TemplateColumn<EnchantedItem>(
                    "EditorID",
                    new FuncDataTemplate<EnchantedItem>((r, _) => new TextBlock {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = r?.EditorID
                    }),
                    options: new TemplateColumnOptions<EnchantedItem> {
                        CompareAscending = (x, y) => x?.EditorID.CompareTo(y?.EditorID, StringComparison.OrdinalIgnoreCase) ?? 0,
                        CompareDescending = (x, y) => y?.EditorID.CompareTo(x?.EditorID, StringComparison.OrdinalIgnoreCase) ?? 0,
                    }
                ),
                new TemplateColumn<EnchantedItem>(
                    "Enchantment",
                    new FuncDataTemplate<EnchantedItem>((x, _) => new TextBlock {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = x?.Enchantment.EditorID
                    }),
                    options: new TemplateColumnOptions<EnchantedItem> {
                        CompareAscending = (x, y) => x?.Enchantment.EditorID.CompareTo(y?.Enchantment.EditorID, StringComparison.OrdinalIgnoreCase) ?? 0,
                        CompareDescending = (x, y) => y?.Enchantment.EditorID.CompareTo(x?.Enchantment.EditorID, StringComparison.OrdinalIgnoreCase) ?? 0,
                    }
                ),
                new TemplateColumn<EnchantedItem>(
                    "Name",
                    new FuncDataTemplate<EnchantedItem>((x, _) => new TextBlock {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = x?.Name
                    }),
                    options: new TemplateColumnOptions<EnchantedItem> {
                        CompareAscending = (x, y) => x?.Name.CompareTo(y?.Name, StringComparison.OrdinalIgnoreCase) ?? 0,
                        CompareDescending = (x, y) => y?.Name.CompareTo(x?.Name, StringComparison.OrdinalIgnoreCase) ?? 0,
                    }
                ),
                new TemplateColumn<EnchantedItem>(
                    "Level",
                    new FuncDataTemplate<EnchantedItem>((x, _) => new TextBlock {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = x?.EnchantmentLevel.ToString()
                    }),
                    options: new TemplateColumnOptions<EnchantedItem> {
                        CompareAscending = (x, y) => x?.EnchantmentLevel.CompareTo(y?.EnchantmentLevel) ?? 0,
                        CompareDescending = (x, y) => y?.EnchantmentLevel.CompareTo(x?.EnchantmentLevel) ?? 0,
                    }
                ),
            }
        };

        this.WhenAnyValue(x => x.DefinitionsFolderPath)
            .NotNull()
            .Subscribe(path => {
                EnchantmentsDefinitions.LoadOptimized(GetDefinitions(path));
                stateRepository.Update(
                    memento => memento is null
                        ? new LeveledListMemento(string.Empty, path)
                        : memento with { EnchantmentsFolderPath = path },
                    enchantmentsMemento.Value is null
                        ? Guid.NewGuid()
                        : enchantmentsMemento.Key);
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedEnchantmentItems)
            .CombineLatest(ModPickerVM.SelectedModChanged, (def, mod) => (Definitions: def, SelectedMod: mod))
            .ThrottleShort()
            .ObserveOnTaskpool()
            .Subscribe(x => UpdateEnchantmentsShowcase(x.Definitions, x.SelectedMod))
            .DisposeWith(this);
    }

    private void GenerateEnchantedItems() {
        if (SelectedEnchantmentItems is null) return;
        if (ModPickerVM.SelectedMod is null) return;

        var selectedMod = _editorEnvironment.ResolveMod(ModPickerVM.SelectedMod.ModKey);
        if (selectedMod is null) return;

        _implementer.ImplementEnchantments(_editorEnvironment.ActiveMod, _enchantedItems);
    }

    private void UpdateEnchantmentsShowcase(IReadOnlyList<ExtendedEnchantmentItem>? selectedLists, OrderedModItem? selectedMod) {
        var mod = _editorEnvironment.ResolveMod(selectedMod?.ModKey);
        if (mod is null || selectedLists is null) {
            Dispatcher.UIThread.Post(() => _enchantedItems.Clear());
            return;
        }

        try {
            _isBusy.OnNext(true);
            var generatedLists = new List<EnchantedItem>();
            foreach (var enchantmentsDefinition in selectedLists.Distinct()) {
                var enchantedItems = _generator.Generate(
                    enchantmentsDefinition.EnchantmentsDefinition.Type,
                    enchantmentsDefinition.EnchantmentItem,
                    mod);
                generatedLists.AddRange(enchantedItems);
            }

            Dispatcher.UIThread.Post(() => {
                _enchantedItems.LoadOptimized(generatedLists);
                _isBusy.OnNext(false);
            });
        } catch (Exception e) {
            Dispatcher.UIThread.Post(() => {
                _enchantedItems.Clear();
                _isBusy.OnNext(false);
            });
            _logger.Here().Error(e,
                "Failed to generate leveled lists for {ListDefinition}",
                string.Join(',', selectedLists.Select(x => x.FileName)));
        }
    }

    private IEnumerable<ExtendedEnchantmentItem> GetDefinitions(string directoryPath) {
        if (!_fileSystem.Directory.Exists(directoryPath)) yield break;

        var deserializer = new DeserializerBuilder()
            .WithTypeConverter(new FormKeyYamlTypeConverter())
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();

        foreach (var file in _fileSystem.Directory.EnumerateFiles(directoryPath, "*.yaml", SearchOption.AllDirectories)) {
            using var fileStream = _fileSystem.File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = _fileSystem.Path.GetFileNameWithoutExtension(file);

            EnchantmentsDefinition enchantmentsDefinition;
            try {
                enchantmentsDefinition = deserializer.Deserialize<EnchantmentsDefinition>(new StreamReader(fileStream));
            } catch (Exception) {
                continue;
            }

            foreach (var enchantment in enchantmentsDefinition.Enchantments.Values) {
                yield return new ExtendedEnchantmentItem(file, fileName, enchantment, enchantmentsDefinition);
            }
        }
    }
}
