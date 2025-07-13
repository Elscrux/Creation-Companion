using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public class MajorRecordExtraColumns : ExtraColumns<IMajorRecordGetter> {
    public override IEnumerable<ExtraColumn> CreateColumns() {
        yield return new ExtraColumn(
            new DataGridTextColumn {
                Header = "EditorID",
                Binding = new Binding(nameof(IReferencedRecord.Record) + '.' + nameof(IReferencedRecord.Record.EditorID), BindingMode.OneWay),
                CanUserSort = true,
                CustomSortComparer = ReferencedRecordComparers.EditorIDComparer,
                Width = new DataGridLength(200),
            },
            220);

        yield return new ExtraColumn(
            new DataGridTextColumn {
                Header = "FormKey",
                Binding = new Binding(nameof(IReferencedRecord.Record) + '.' + nameof(IReferencedRecord.Record.FormKey), BindingMode.OneWay),
                CanUserSort = true,
                CustomSortComparer = ReferencedRecordComparers.FormKeyComparer,
                Width = new DataGridLength(100),
            },
            210);

        yield return new ExtraColumn(
            new DataGridTemplateColumn {
                Header = "References",
                CellTemplate = IUntypedExtraColumns.GetTextCellTemplate(r =>
                    r.ReferenceCount
                        .Select(x => x.ToString())
                        .ToBinding()),
                CanUserSort = true,
                CustomSortComparer = ReferencedRecordComparers.ReferenceCountComparer,
            },
            200);
    }
}
