using Avalonia.Controls;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.List.ExtraColumns;

public class BookExtraColumns : ExtraColumns<IBookGetter> {
    private readonly Func<IMajorRecordGetter, object?> _bookTypeSelector =
        record => record is IBookGetter book
            ? book.Teaches switch {
                IBookSkillGetter => "Skill Book",
                IBookSpellGetter => "Spell Tome",
                IBookTeachesNothingGetter => "Book",
                _ => throw new InvalidOperationException()
            }
            : null;

    public override IEnumerable<ExtraColumn> CreateColumns() {
        yield return new ExtraColumn(new DataGridTemplateColumn {
                Header = "Type",
                CellTemplate = IUntypedExtraColumns.GetTextCellTemplate(_bookTypeSelector),
                CanUserSort = true,
                CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _bookTypeSelector(x.Record)),
            },
            150);
    }
}
