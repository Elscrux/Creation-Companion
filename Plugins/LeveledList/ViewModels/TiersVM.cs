using System.Collections;
using System.Reactive.Linq;
using Avalonia.Threading;
using CreationEditor;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using LeveledList.Model;
using LeveledList.Model.Tier;
using LeveledList.Services;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace LeveledList.ViewModels;

public sealed partial class TiersVM : ValidatableViewModel {
    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly IRecordListVMBuilder _recordListVMBuilder;
    private readonly ITierController _tierController;
    private IExtraColumnsBuilder _extraColumnsBuilder;
    public IObservableCollection<TierItem> Tiers { get; } = new ObservableCollectionExtended<TierItem>();
    public IObservableCollection<TierAliasItem> TierAliases { get; } = new ObservableCollectionExtended<TierAliasItem>();

    public IReadOnlyList<ListRecordType> TierRecordTypes { get; } = Enum.GetValues<ListRecordType>();

    [Reactive] public partial ListRecordType SelectedTierType { get; set; } = ListRecordType.Armor;
    [Reactive] public partial TierItem? SelectedTier { get; set; }
    [Reactive] public partial IRecordListVM? RecordListVM { get; set; }

    public TiersVM(
        ILinkCacheProvider linkCacheProvider,
        IRecordListVMBuilder recordListVMBuilder,
        IExtraColumnsBuilder extraColumnsBuilder,
        ITierController tierController) {
        _linkCacheProvider = linkCacheProvider;
        _recordListVMBuilder = recordListVMBuilder;
        _extraColumnsBuilder = extraColumnsBuilder;
        _tierController = tierController;
        Task.Run(() => RecordListVM = GetRecordListVM());

        this.WhenAnyValue(x => x.SelectedTierType)
            .Select(tierController.GetTierDefinitions)
            .ObserveOnGui()
            .Subscribe(UpdateTierDefinitions)
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedTier)
            .ObserveOnTaskpool()
            .Subscribe(RefreshRecordList)
            .DisposeWith(this);
    }

    private void UpdateTierDefinitions(TierDefinitions? tierDefinitions) {
        var tierItems = tierDefinitions?.Tiers
            .Select(tier => new TierItem {
                Identifier = tier.Key,
                EnchantmentLevels = string.Join(", ", tier.Value.Levels),
            })
            .ToArray();

        var tierAliases = tierDefinitions?.TierAliases?
            .Select(alias => new TierAliasItem {
                Alias = tierItems?.FirstOrDefault(x => x.Identifier == alias.Key),
                Original = tierItems?.FirstOrDefault(x => x.Identifier == alias.Value),
            })
            .ToArray();

        SelectedTier = tierItems?.FirstOrDefault();
        Tiers.LoadOptimized(tierItems ?? []);
        TierAliases.LoadOptimized(tierAliases ?? []);
    }

    private void RefreshRecordList() {
        var vm = GetRecordListVM();
        Dispatcher.UIThread.Post(() => RecordListVM = vm);
    }

    [ReactiveCommand]
    private void AddTier(string tierName) {
        var tier = new TierItem {
            Identifier = tierName
        };
        if (Tiers.Contains(tier)) return;

        Tiers.Add(tier);
    }

    [ReactiveCommand]
    private void RemoveTier(IList tiers) {
        Tiers.RemoveMany(tiers.OfType<TierItem>());
    }

    [ReactiveCommand]
    private void AddTierAlias() {
        var tierAlias = new TierAliasItem();
        if (TierAliases.Contains(tierAlias)) return;

        TierAliases.Add(tierAlias);
    }

    [ReactiveCommand]
    private void RemoveTierAlias(IList tiers) {
        TierAliases.RemoveMany(tiers.OfType<TierAliasItem>());
    }

    [ReactiveCommand]
    private void SaveTiers() {
        var tierIdentifiers = Tiers
            .Where(x => !string.IsNullOrEmpty(x.Identifier))
            .ToDictionary(x => x.Identifier, x => new TierData(x.GetEnchantmentLevels()));

        var tierAliases = TierAliases
            .Where(x => !string.IsNullOrEmpty(x.Original?.Identifier) && !string.IsNullOrEmpty(x.Alias?.Identifier))
            .ToDictionary(x => x.Alias!.Identifier, x => x.Original!.Identifier);

        _tierController.SetTiers(SelectedTierType, tierIdentifiers, tierAliases);

        RecordListVM = GetRecordListVM();
    }

    private IRecordListVM GetRecordListVM() {
        var tiers = _tierController
            .GetTiers(SelectedTierType)
            .Select(x => x.Key)
            .ToHashSet();

        var records = _tierController.GetAllRecordTiers()
            .Where(x => tiers.Contains(x.Value.TierIdentifier))
            .Where(x => _linkCacheProvider.LinkCache.TryResolve(x.Key, out _));

        if (SelectedTier is not null) {
            records = records
                .Where(x => x.Value.TierIdentifier == SelectedTier.Identifier);
        }

        var formLinkInformation = records
            .Select(x => x.Key);

        foreach (var recordType in SelectedTierType.GetRecordTypes()) {
            _extraColumnsBuilder = _extraColumnsBuilder.AddRecordType(recordType);
        }

        return Dispatcher.UIThread.Invoke(() => _recordListVMBuilder
            .WithExtraColumns(_extraColumnsBuilder)
            .BuildWithSource(formLinkInformation)
            .DisposeWith(this));
    }
}
