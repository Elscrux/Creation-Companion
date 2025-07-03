using Avalonia.Controls;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services.Record.List.ExtraColumns;

public sealed class EnchantableFeaturesExtraColumn(IFeatureProvider featureProvider) : IAutoAttachingExtraColumns {
    private readonly Func<IMajorRecordGetter, object?> _schoolOfMagicSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.SchoolOfMagic).Selector;

    private readonly Func<IMajorRecordGetter, object?> _enchantmentSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.Enchantment).Selector;

    public IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Enchantment",
                    CellTemplate = FeaturesExtraColumn.GetCellTemplate(_enchantmentSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _enchantmentSelector(x.Record)),
                },
                142),
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "School of Magic",
                    CellTemplate = FeaturesExtraColumn.GetCellTemplate(_schoolOfMagicSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _schoolOfMagicSelector(x.Record)),
                },
                141),
        ];
    }

    public bool CanAttachTo(Type type) => type.IsAssignableTo(typeof(IEnchantableGetter));
}
