using Avalonia.Controls;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services.Record.List.ExtraColumns;

public sealed class WeaponFeaturesExtraColumn(IFeatureProvider featureProvider) : ExtraColumns<IWeaponGetter> {
    private readonly Func<IMajorRecordGetter, object?> _weaponTypeSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.WeaponType).Selector;

    private readonly Func<IMajorRecordGetter, object?> _magicLevelSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.MagicLevel).Selector;

    public override IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Weapon Type",
                    CellTemplate = IUntypedExtraColumns.GetTextCellTemplate(_weaponTypeSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _weaponTypeSelector(x.Record)),
                },
                147),
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Magic Level",
                    CellTemplate = IUntypedExtraColumns.GetTextCellTemplate(_magicLevelSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _magicLevelSelector(x.Record)),
                },
                140),
        ];
    }
}
