using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Comparer;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns; 

public class MajorRecordExtraColumns : ExtraColumns<IMajorRecordGetter> {
    public override IEnumerable<ExtraColumn> Columns {
        get {
            yield return new ExtraColumn(
                new DataGridTextColumn {
                    Header = "EditorID",
                    Binding = new Binding("Record.EditorID", BindingMode.OneWay),
                    CanUserSort = true,
                    CustomSortComparer = RecordComparers.EditorIDComparer,
                    Width = new DataGridLength(200),
                },
                220);
            
            yield return new ExtraColumn(
                new DataGridTextColumn {
                    Header = "FormKey",
                    Binding = new Binding("Record.FormKey", BindingMode.OneWay),
                    CanUserSort = true,
                    CustomSortComparer = RecordComparers.FormKeyComparer,
                    Width = new DataGridLength(100),
                },
                210);

            yield return new ExtraColumn(
                new DataGridTextColumn {
                    Header = "Use Info",
                    Binding = new Binding("References.Count", BindingMode.OneWay),
                    CanUserSort = true,
                    CustomSortComparer = RecordComparers.ReferenceCountComparer,
                    Width = new DataGridLength(90),
                },
                200);
        }
    }
}
