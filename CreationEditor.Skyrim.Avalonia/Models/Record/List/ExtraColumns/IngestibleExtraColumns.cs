using Avalonia.Controls;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.List.ExtraColumns;

public class IngestibleExtraColumns : ExtraColumns<IIngestibleGetter> {
    private readonly Func<IMajorRecordGetter, object?> _potionTypeSelector =
        record => record is IIngestibleGetter ingestible
            ? ingestible.Flags switch {
                var flag when flag.HasFlag(Ingestible.Flag.FoodItem)  => "Food",
                var flag when flag.HasFlag(Ingestible.Flag.Poison) => "Poison",
                _ => "Potion"
            }
            : null;

    public override IEnumerable<ExtraColumn> CreateColumns() {
        yield return new ExtraColumn(new DataGridTemplateColumn {
                Header = "Type",
                CellTemplate = IUntypedExtraColumns.GetTextCellTemplate(_potionTypeSelector),
                CanUserSort = true,
                CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _potionTypeSelector(x.Record)),
            },
            150);
    }
}
