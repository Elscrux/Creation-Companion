using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using LeveledList.Model.Tier;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace LeveledList.ViewModels;

public sealed partial class TiersVM : ValidatableViewModel {
    public IObservableCollection<TierItem> Tiers { get; } = new ObservableCollectionExtended<TierItem>();

    public List<Type> TierRecordTypes { get; } = [
        typeof(IArmorGetter),
        typeof(IWeaponGetter),
    ];

    [Reactive] public partial Type SelectedTierType { get; set; } = typeof(IArmorGetter);
    [Reactive] public partial TierItem? SelectedTier { get; set; }
    [Reactive] public partial IRecordListVM RecordListVM { get; set; }

    public ReactiveCommand<Unit, Unit> SaveTiers { get; }
    public ReactiveCommand<string, Unit> AddTier { get; }
    public ReactiveCommand<IList, Unit> RemoveTier { get; }

    public TiersVM(
        ILinkCacheProvider linkCacheProvider,
        IRecordListVMBuilder recordListVMBuilder,
        IExtraColumnsBuilder extraColumnsBuilder,
        ITierController tierController) {
        RecordListVM = GetRecordListVM();

        this.WhenAnyValue(x => x.SelectedTierType)
            .Select(tierController.GetTiers)
            .Subscribe(tiers => {
                SelectedTier = null;
                Tiers.Load(tiers
                    .Select(tier => new TierItem { Identifier = tier }));

                RecordListVM = GetRecordListVM();
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedTier)
            .Subscribe(_ => {
                RecordListVM = GetRecordListVM();
            })
            .DisposeWith(this);

        SaveTiers = ReactiveCommand.Create(() => {
            var tierIdentifiers = Tiers
                .Select(x => x.Identifier)
                .Where(x => !x.IsNullOrEmpty());

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
            var records = tierController.GetAllRecordTiers()
                .Where(x => linkCacheProvider.LinkCache.TryResolve(x.Key, SelectedTierType, out _));

            if (SelectedTier is not null) {
                records = records
                    .Where(x => x.Value.TierIdentifier == SelectedTier.Identifier);
            }

            var formLinkInformation = records
                .Select(x => new FormLinkInformation(x.Key, SelectedTierType));

            return recordListVMBuilder
                .WithExtraColumns(extraColumnsBuilder
                    .AddRecordType(SelectedTierType))
                .BuildWithSource(formLinkInformation)
                .DisposeWith(this);
        }
    }
}
