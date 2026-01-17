using System.Collections;
using System.Reactive;
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
    public IObservableCollection<TierItem> Tiers { get; } = new ObservableCollectionExtended<TierItem>();
    public IObservableCollection<TierAliasItem> TierAliases { get; } = new ObservableCollectionExtended<TierAliasItem>();

    public IReadOnlyList<ListRecordType> TierRecordTypes { get; } = Enum.GetValues<ListRecordType>();

    [Reactive] public partial ListRecordType SelectedTierType { get; set; } = ListRecordType.Armor;
    [Reactive] public partial TierItem? SelectedTier { get; set; }
    [Reactive] public partial IRecordListVM? RecordListVM { get; set; }

    public ReactiveCommand<Unit, Unit> SaveTiers { get; }
    public ReactiveCommand<string, Unit> AddTier { get; }
    public ReactiveCommand<IList, Unit> RemoveTier { get; }
    public ReactiveCommand<Unit, Unit> AddTierAlias { get; }
    public ReactiveCommand<IList, Unit> RemoveTierAlias { get; }

    public TiersVM(
        ILinkCacheProvider linkCacheProvider,
        IRecordListVMBuilder recordListVMBuilder,
        IExtraColumnsBuilder extraColumnsBuilder,
        ITierController tierController) {
        Task.Run(() => RecordListVM = GetRecordListVM());

        this.WhenAnyValue(x => x.SelectedTierType)
            .Select(tierController.GetTierDefinitions)
            .ObserveOnGui()
            .Subscribe(tierDefinitions => {
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
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedTier)
            .ObserveOnTaskpool()
            .Subscribe(_ => {
                var vm = GetRecordListVM();
                Dispatcher.UIThread.Post(() => RecordListVM = vm);
            })
            .DisposeWith(this);

        SaveTiers = ReactiveCommand.Create(() => {
            var tierIdentifiers = Tiers
                .Where(x => !string.IsNullOrEmpty(x.Identifier))
                .ToDictionary(x => x.Identifier, x => new TierData(x.GetEnchantmentLevels()));

            var tierAliases = TierAliases
                .Where(x => !string.IsNullOrEmpty(x.Original?.Identifier) && !string.IsNullOrEmpty(x.Alias?.Identifier))
                .ToDictionary(x => x.Alias!.Identifier, x => x.Original!.Identifier);

            tierController.SetTiers(SelectedTierType, tierIdentifiers, tierAliases);

            RecordListVM = GetRecordListVM();
        });

        AddTier = ReactiveCommand.Create<string>(tierName => {
            var tier = new TierItem { Identifier = tierName };
            if (Tiers.Contains(tier)) return;

            Tiers.Add(tier);
        });

        RemoveTier = ReactiveCommand.Create<IList>(tiers => {
            Tiers.RemoveMany(tiers.OfType<TierItem>());
        });

        AddTierAlias = ReactiveCommand.Create(() => {
            var tierAlias = new TierAliasItem();
            if (TierAliases.Contains(tierAlias)) return;

            TierAliases.Add(tierAlias);
        });

        RemoveTierAlias = ReactiveCommand.Create<IList>(tiers => {
            TierAliases.RemoveMany(tiers.OfType<TierAliasItem>());
        });

        IRecordListVM GetRecordListVM() {
            var tiers = tierController
                .GetTiers(SelectedTierType)
                .Select(x => x.Key)
                .ToHashSet();

            var records = tierController.GetAllRecordTiers()
                .Where(x => tiers.Contains(x.Value.TierIdentifier))
                .Where(x => linkCacheProvider.LinkCache.TryResolve(x.Key, out _));

            if (SelectedTier is not null) {
                records = records
                    .Where(x => x.Value.TierIdentifier == SelectedTier.Identifier);
            }

            var formLinkInformation = records
                .Select(x => x.Key);

            foreach (var recordType in SelectedTierType.GetRecordTypes()) {
                extraColumnsBuilder = extraColumnsBuilder.AddRecordType(recordType);
            }

            return Dispatcher.UIThread.Invoke(() => recordListVMBuilder
                .WithExtraColumns(extraColumnsBuilder)
                .BuildWithSource(formLinkInformation)
                .DisposeWith(this));
        }
    }
}
