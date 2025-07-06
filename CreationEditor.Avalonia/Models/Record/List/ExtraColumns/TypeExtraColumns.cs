using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Services.Mutagen.References.Record;
namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public sealed class TypeExtraColumns : IUntypedExtraColumns {
    public IEnumerable<ExtraColumn> CreateColumns() {
        yield return new ExtraColumn(
            new DataGridTextColumn {
                Header = "Type",
                Binding = new Binding(nameof(IReferencedRecord.RecordTypeName), BindingMode.OneWay),
                CanUserSort = true,
                CustomSortComparer = ReferencedRecordComparers.TypeComparer,
            },
            150);
    }
}
