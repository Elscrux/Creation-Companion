using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Services.Mutagen.References.Record;
using Mutagen.Bethesda.Plugins.Aspects;
namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public sealed class WeightValueExtraColumns : ExtraColumns<IWeightValueGetter> {
    public override IEnumerable<ExtraColumn> CreateColumns() {
        yield return new ExtraColumn(
            new DataGridTextColumn {
                Header = "Weight",
                Binding = new Binding(nameof(IReferencedRecord.Record) + '.' + nameof(IWeightValueGetter.Weight), BindingMode.OneWay),
                CanUserSort = true,
                CustomSortComparer = ReferencedRecordComparers.WeightComparer,
                Width = new DataGridLength(85),
            },
            10);

        yield return new ExtraColumn(
            new DataGridTextColumn {
                Header = "Value",
                Binding = new Binding(nameof(IReferencedRecord.Record) + '.' + nameof(IWeightValueGetter.Value), BindingMode.OneWay),
                CanUserSort = true,
                CustomSortComparer = ReferencedRecordComparers.ValueComparer,
                Width = new DataGridLength(75),
            },
            11);
    }
}
