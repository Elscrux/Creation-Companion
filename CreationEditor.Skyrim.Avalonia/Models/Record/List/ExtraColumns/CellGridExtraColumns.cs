using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Avalonia.Resources.Comparer;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.List.ExtraColumns;

public sealed class CellGridExtraColumns : IUntypedExtraColumns {
    public IEnumerable<ExtraColumn> CreateColumns() {
        yield return new ExtraColumn(
            new DataGridTextColumn {
                Header = "Grid",
                Binding = new Binding(nameof(IReferencedRecord.Record) + '.' + nameof(ICellGetter.Grid) + '.'  +  nameof(ICellGetter.Grid.Point), BindingMode.OneWay),
                CanUserSort = true,
                CustomSortComparer = SkyrimRecordComparers.CellGridComparer,
                Width = new DataGridLength(100),
            },
            150);
    }
}
