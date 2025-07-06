using Avalonia.Controls;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services.Record.List.ExtraColumns;

public sealed class AmmunitionFeaturesExtraColumn(IFeatureProvider featureProvider) : ExtraColumns<IAmmunitionGetter> {
    private readonly Func<IMajorRecordGetter, object?> _ammunitionTypeSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.AmmunitionType).Selector;

    public override IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Ammunition Type",
                    CellTemplate = IUntypedExtraColumns.GetTextCellTemplate(_ammunitionTypeSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _ammunitionTypeSelector(x.Record)),
                },
                146),
        ];
    }
}
