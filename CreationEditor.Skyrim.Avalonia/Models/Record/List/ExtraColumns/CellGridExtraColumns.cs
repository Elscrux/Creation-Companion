using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Skyrim.Avalonia.Resources.Comparer;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.List.ExtraColumns;

public class CellGridExtraColumns : IUntypedExtraColumns {
    public IEnumerable<ExtraColumn> CreateColumns() {
        yield return new ExtraColumn(
            new DataGridTextColumn {
                Header = "Grid",
                Binding = new Binding("Record.Grid.Point", BindingMode.OneWay),
                CanUserSort = true,
                CustomSortComparer = SkyrimRecordComparers.CellGridComparer,
                Width = new DataGridLength(100),
            },
            150);
    }
}
