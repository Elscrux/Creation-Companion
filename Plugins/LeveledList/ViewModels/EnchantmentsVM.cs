using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Threading;
using CreationEditor;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.State;
using DynamicData;
using DynamicData.Binding;
using LeveledList.Model;
using LeveledList.Model.Enchantments;
using LeveledList.Services.Enchantments;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace LeveledList.ViewModels;

public sealed partial class EnchantmentsVM : ValidatableViewModel {
    private readonly ILogger _logger;
    private readonly EnchantmentsGenerator _generator;
    private readonly EnchantmentsImplementer _implementer;

    public GenerationConfigurationVM GenerationConfig { get; }

    private readonly ObservableCollectionExtended<EnchantedItem> _enchantedItems = [];
    public ReadOnlyObservableCollection<EnchantedItem> EnchantedItems { get; }
    public IObservableCollection<ExtendedEnchantmentItem> EnchantmentsDefinitions { get; } = new ObservableCollectionExtended<ExtendedEnchantmentItem>();

    [Reactive] public partial IReadOnlyList<ExtendedEnchantmentItem>? SelectedDefinitions { get; set; }

    public FlatTreeDataGridSource<EnchantedItem> EnchantmentsSource { get; }
    public ReactiveCommand<Unit, Unit> GenerateEnchantments { get; }
    public ReactiveCommand<Unit, Unit> ReloadDefinitions { get; }

    public EnchantmentsVM(
        IStateRepositoryFactory<LeveledListMemento, LeveledListMemento, Guid> stateRepositoryFactory,
        ILogger logger,
        ISearchFilter searchFilter,
        EnchantmentsGenerator generator,
        GenerationConfigurationVM generationConfig,
        EnchantmentsImplementer implementer) {
        _logger = logger;
        _generator = generator;
        _implementer = implementer;

        GenerationConfig = generationConfig;
        GenerationConfig.ShowNameReplacement = true;

        var stateRepository = stateRepositoryFactory.Create("Enchantments");
        var enchantmentsMemento = stateRepository.LoadAllWithIdentifier().FirstOrDefault();
        Guid? enchantmentsMementoGuid = enchantmentsMemento.Value is null ? null : enchantmentsMemento.Key;

        GenerationConfig.DefinitionsFolderPath = enchantmentsMemento.Value?.EnchantmentsFolderPath;

        var filter = GenerationConfig.WhenAnyValue(x => x.FilterText)
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
        ReloadDefinitions = ReactiveCommand.Create(() => {
            if (GenerationConfig.DefinitionsFolderPath is null) return;

            LoadDefinitions(GenerationConfig.DefinitionsFolderPath);
        });

        EnchantmentsSource = new FlatTreeDataGridSource<EnchantedItem>(EnchantedItems) {
            Columns = {
                new TemplateColumn<EnchantedItem>(
                    "Exists",
                    new FuncDataTemplate<EnchantedItem>((x, _) => new TextBlock {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = x?.ExistingEnchanted is not null ? "✅" : string.Empty
                    }),
                    options: new TemplateColumnOptions<EnchantedItem> {
                        CompareAscending = (x, y) => {
                            if (x is null && y is null) return 0;
                            if (x is null) return -1;
                            if (y is null) return 1;

                            var xExists = x.ExistingEnchanted is not null;
                            var yExists = y.ExistingEnchanted is not null;
                            return xExists.CompareTo(yExists);
                        },
                        CompareDescending = (x, y) => {
                            if (x is null && y is null) return 0;
                            if (x is null) return 1;
                            if (y is null) return -1;

                            var xExists = x.ExistingEnchanted is not null;
                            var yExists = y.ExistingEnchanted is not null;
                            return yExists.CompareTo(xExists);
                        }
                    }
                ),
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

        GenerationConfig.WhenAnyValue(x => x.DefinitionsFolderPath)
            .NotNull()
            .Subscribe(path => {
                LoadDefinitions(path);
                stateRepository.Update(
                    memento => memento is null
                        ? new LeveledListMemento(string.Empty, path)
                        : memento with { EnchantmentsFolderPath = path },
                    enchantmentsMementoGuid ??= Guid.NewGuid());
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedDefinitions)
            .CombineLatest(
                GenerationConfig.ModPicker.SelectedMods,
                GenerationConfig.RecordPrefixContent.RecordPrefixService.PrefixChanged.ThrottleMedium(),
                GenerationConfig.EditorIdReplacement.WhenAnyValue(x => x.Pattern, x => x.Replacement).ThrottleMedium(),
                GenerationConfig.NameReplacement.WhenAnyValue(x => x.Pattern, x => x.Replacement).ThrottleMedium(),
                (def, mods, _, _, _) => (Definitions: def, SelectedMods: mods))
            .ThrottleShort()
            .ObserveOnTaskpool()
            .Subscribe(x => UpdateEnchantmentsShowcase(x.Definitions, x.SelectedMods))
            .DisposeWith(this);
    }

    private void LoadDefinitions(string directoryPath) {
        EnchantmentsDefinitions.LoadOptimized(GetDefinitions(directoryPath));
    }

    private void GenerateEnchantedItems() {
        if (SelectedDefinitions is null) return;

        _implementer.ImplementEnchantments(GenerationConfig.EditorEnvironment.ActiveMod, _enchantedItems);
    }

    private void UpdateEnchantmentsShowcase(IReadOnlyList<ExtendedEnchantmentItem>? selectedLists, IReadOnlyCollection<OrderedModItem> selectedMods) {
        var mods = GenerationConfig.EditorEnvironment.ResolveMods(selectedMods.Select(x => x.ModKey)).ToArray();
        if (mods.Length == 0 || selectedLists is null) {
            Dispatcher.UIThread.Post(() => _enchantedItems.Clear());
            return;
        }

        try {
            GenerationConfig.SetBusy(true);
            var generatedLists = new List<EnchantedItem>();
            foreach (var enchantmentsDefinition in selectedLists.Distinct()) {
                var enchantedItems = _generator.Generate(
                    enchantmentsDefinition.EnchantmentsDefinition.Type,
                    enchantmentsDefinition.EnchantmentItem,
                    mods,
                    GenerationConfig.EditorIdReplacement.Replace,
                    GenerationConfig.NameReplacement is {} rep ? rep.Replace : s => s);
                generatedLists.AddRange(enchantedItems);
            }

            Dispatcher.UIThread.Post(() => {
                _enchantedItems.LoadOptimized(generatedLists);
                GenerationConfig.SetBusy(false);
            });
        } catch (Exception e) {
            Dispatcher.UIThread.Post(() => {
                _enchantedItems.Clear();
                GenerationConfig.SetBusy(false);
            });
            _logger.Here().Error(e,
                "Failed to generate enchantments for {ListDefinition}",
                string.Join(',', selectedLists.Select(x => x.FileName)));
        }
    }

    private IEnumerable<ExtendedEnchantmentItem> GetDefinitions(string directoryPath) {
        return GenerationConfig.GetDefinitionsFromYaml<ExtendedEnchantmentItem>(
            directoryPath,
            (deserializer, reader, file, fileName) => {
                var enchantmentsDefinition = deserializer.Deserialize<EnchantmentsDefinition>(reader);
                return enchantmentsDefinition.Enchantments.Values.Select(enchantment =>
                    new ExtendedEnchantmentItem(file, fileName, enchantment, enchantmentsDefinition));
            });
    }
}
