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
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace LeveledList.ViewModels;

public sealed partial class TierItem : ReactiveObject {
    [Reactive] public partial TierIdentifier Identifier { get; set; } = string.Empty;
}

public sealed partial class LeveledListVM : ValidatableViewModel {
    public List<Type> ItemTypes { get; } = [
        typeof(IArmorGetter),
        typeof(IWeaponGetter),
    ];

    [Reactive] public partial Type SelectedItemType { get; set; } = typeof(IArmorGetter);
    [Reactive] public partial TierItem? SelectedTier { get; set; }
    [Reactive] public partial IRecordListVM RecordListVM { get; set; }

    public IObservableCollection<TierItem> Tiers { get; } = new ObservableCollectionExtended<TierItem>();
    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<string, Unit> AddTier { get; }
    public ReactiveCommand<IList, Unit> RemoveTier { get; }

    public LeveledListVM(
        ILinkCacheProvider linkCacheProvider,
        IRecordListVMBuilder recordListVMBuilder,
        IExtraColumnsBuilder extraColumnsBuilder,
        ITierController tierController) {
        RecordListVM = GetRecordListVM();

        this.WhenAnyValue(x => x.SelectedItemType)
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

        Save = ReactiveCommand.Create(() => {
            var tierIdentifiers = Tiers
                .Select(x => x.Identifier)
                .Where(x => !x.IsNullOrEmpty());

            tierController.SetTiers(SelectedItemType, tierIdentifiers);

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
                .Where(x => linkCacheProvider.LinkCache.TryResolve(x.Key, SelectedItemType, out _));
            
            if (SelectedTier is not null) {
                records = records
                    .Where(x => x.Value.TierIdentifier == SelectedTier.Identifier);
            }

            var formLinkInformation = records
                .Select(x => new FormLinkInformation(x.Key, SelectedItemType));

            return recordListVMBuilder
                .WithExtraColumns(extraColumnsBuilder
                    .AddRecordType(SelectedItemType))
                .BuildWithSource(formLinkInformation)
                .DisposeWith(this);
        }
    }
}
