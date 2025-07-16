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

    public IReadOnlyList<ListRecordType> TierRecordTypes { get; } = Enum.GetValues<ListRecordType>();

    [Reactive] public partial ListRecordType SelectedTierType { get; set; } = ListRecordType.Armor;
    [Reactive] public partial TierItem? SelectedTier { get; set; }
    [Reactive] public partial IRecordListVM? RecordListVM { get; set; }

    public ReactiveCommand<Unit, Unit> SaveTiers { get; }
    public ReactiveCommand<string, Unit> AddTier { get; }
    public ReactiveCommand<IList, Unit> RemoveTier { get; }

    public TiersVM(
        ILinkCacheProvider linkCacheProvider,
        IRecordListVMBuilder recordListVMBuilder,
        IExtraColumnsBuilder extraColumnsBuilder,
        ITierController tierController) {
        Task.Run(() => RecordListVM = GetRecordListVM());

        this.WhenAnyValue(x => x.SelectedTierType)
            .Select(tierController.GetTiers)
            .ObserveOnGui()
            .Subscribe(tiers => {
                var tierItems = tiers
                    .Select(tier => new TierItem {
                        Identifier = tier.Key,
                        EnchantmentLevels = string.Join(", ", tier.Value.Levels),
                    })
                    .ToArray();
                SelectedTier = tierItems.FirstOrDefault();
                Tiers.LoadOptimized(tierItems);
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

            tierController.SetTiers(SelectedTierType, tierIdentifiers);

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
