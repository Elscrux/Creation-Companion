using Avalonia.Controls;
using Avalonia.Data;
using Mutagen.Bethesda.Plugins.Aspects;
namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public sealed class WeightValueExtraColumns : ExtraColumns<IWeightValue, IWeightValueGetter> {
    public override IEnumerable<ExtraColumn> Columns {
        get {
            yield return new ExtraColumn(
                new DataGridTextColumn {
                    Header = "Weight",
                    Binding = new Binding("Record.Weight", BindingMode.OneWay),
                    CanUserSort = true,
                    Width = new DataGridLength(85),
                }, 10);
            yield return new ExtraColumn(
                new DataGridTextColumn {
                    Header = "Value",
                    Binding = new Binding("Record.Value", BindingMode.OneWay),
                    CanUserSort = true,
                    Width = new DataGridLength(75),
                }, 11);
        }
    }
}
