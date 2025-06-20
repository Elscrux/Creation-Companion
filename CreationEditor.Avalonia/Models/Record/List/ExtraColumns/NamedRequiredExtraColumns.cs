using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Services.Mutagen.References.Record;
using Mutagen.Bethesda.Plugins.Aspects;
namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public sealed class NamedRequiredExtraColumns : ExtraColumns<INamedRequiredGetter> {
    public override IEnumerable<ExtraColumn> CreateColumns() {
        yield return new ExtraColumn(
            new DataGridTextColumn {
                Header = "Name",
                Binding = new Binding(nameof(IReferencedRecord.Record) + '.' + nameof(INamedRequiredGetter.Name), BindingMode.OneWay),
                CanUserSort = true,
                CustomSortComparer = ReferencedRecordComparers.NamedRequiredComparer,
                Width = new DataGridLength(100),
            },
            99);
    }
}
