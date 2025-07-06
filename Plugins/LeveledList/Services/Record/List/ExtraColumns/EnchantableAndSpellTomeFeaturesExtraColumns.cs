using Avalonia.Controls;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services.Record.List.ExtraColumns;

public sealed class SpellTomeFeaturesExtraColumns(IFeatureProvider featureProvider) : ExtraColumns<IBookGetter> {
    private readonly Func<IMajorRecordGetter, object?> _schoolOfMagicSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.SchoolOfMagic).Selector;

    private readonly Func<IMajorRecordGetter, object?> _magicLevelSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.MagicLevel).Selector;

    public override IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "School of Magic",
                    CellTemplate = IUntypedExtraColumns.GetTextCellTemplate(_schoolOfMagicSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _schoolOfMagicSelector(x.Record)),
                },
                141),
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
