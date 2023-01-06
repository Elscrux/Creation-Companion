using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Comparer;
using Mutagen.Bethesda.Plugins.Aspects;
namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public class NamedRequiredExtraColumns : ExtraColumns<INamedRequired, INamedRequiredGetter> {
    public override IEnumerable<ExtraColumn> Columns {
        get {
            yield return new ExtraColumn(
                new DataGridTextColumn {
                    Header = "Name",
                    Binding = new Binding("Record.Name", BindingMode.OneWay),
                    CanUserSort = true,
                    CustomSortComparer = RecordComparers.NamedRequiredComparer,
                    Width = new DataGridLength(100),
                },
                99);
        }
    }
}