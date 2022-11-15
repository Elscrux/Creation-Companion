using Avalonia.Controls;
using Avalonia.Data;
using Mutagen.Bethesda.Plugins.Aspects;
namespace CreationEditor.WPF.Services.Record;

public class WeightValueExtraColumns : ExtraColumns<IWeightValue, IWeightValueGetter> {
    public override IEnumerable<ExtraColumn> Columns {
        get {
            yield return new ExtraColumn(
                new DataGridTextColumn {
                    Header = "Weight",
                    Binding = new Binding("Record.Weight"),
                    CanUserSort = true,
                    Width = new DataGridLength(75),
                }, 10);
            yield return new ExtraColumn(
                new DataGridTextColumn {
                    Header = "Value",
                    Binding = new Binding("Record.Value"),
                    CanUserSort = true,
                    Width = new DataGridLength(75),
                }, 11);
        }
    }
}
