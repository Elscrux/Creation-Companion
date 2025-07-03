using Avalonia.Controls;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services.Record.List.ExtraColumns;

public sealed class ArmorFeaturesExtraColumn(IFeatureProvider featureProvider) : IAutoAttachingExtraColumns {
    private readonly Func<IMajorRecordGetter, object?> _armorTypeSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.ArmorType).Selector;

    private readonly Func<IMajorRecordGetter, object?> _armorSlotSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.ArmorSlot).Selector;

    public IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Armor Type",
                    CellTemplate = IUntypedExtraColumns.GetTextCellTemplate(_armorTypeSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _armorTypeSelector(x.Record)),
                },
                146),
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Armor Slot",
                    CellTemplate = IUntypedExtraColumns.GetTextCellTemplate(_armorSlotSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _armorSlotSelector(x.Record)),
                },
                145),
        ];
    }

    public bool CanAttachTo(Type type) => type.IsAssignableTo(typeof(IArmorGetter));
}
