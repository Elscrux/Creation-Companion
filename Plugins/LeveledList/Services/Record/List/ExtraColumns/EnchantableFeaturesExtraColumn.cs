using Avalonia.Controls;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services.Record.List.ExtraColumns;

public sealed class EnchantableFeaturesExtraColumn(IFeatureProvider featureProvider) : ExtraColumns<IEnchantableGetter> {
    private readonly Func<IMajorRecordGetter, object?> _schoolOfMagicSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.SchoolOfMagic).Selector;

    private readonly Func<IMajorRecordGetter, object?> _enchantmentSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.Enchantment).Selector;

    public override IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Enchantment",
                    CellTemplate = IUntypedExtraColumns.GetTextCellTemplate(_enchantmentSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _enchantmentSelector(x.Record)),
                },
                142),
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "School of Magic",
                    CellTemplate = IUntypedExtraColumns.GetTextCellTemplate(_schoolOfMagicSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _schoolOfMagicSelector(x.Record)),
                },
                141),
        ];
    }
}
