using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Comparer;
namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public class TypeExtraColumns : IUntypedExtraColumns {
    public IEnumerable<ExtraColumn> Columns {
        get {
            yield return new ExtraColumn(
                new DataGridTextColumn {
                    Header = "Type",
                    Binding = new Binding("RecordTypeName", BindingMode.OneWay),
                    CanUserSort = true,
                    CustomSortComparer = RecordComparers.TypeComparer,
                },
                150);
        }
    }
}
