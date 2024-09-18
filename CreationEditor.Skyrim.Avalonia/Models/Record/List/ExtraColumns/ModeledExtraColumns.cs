using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Skyrim.Avalonia.Resources.Comparer;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.List.ExtraColumns;

public class ModeledExtraColumns : ExtraColumns<IModeledGetter> {
    public override IEnumerable<ExtraColumn> CreateColumns() {
        yield return new ExtraColumn(new DataGridTextColumn {
                Header = "Model",
                Binding = new Binding("Record.Model.File.DataRelativePath", BindingMode.OneWay),
                CanUserSort = true,
                Width = new DataGridLength(200),
                CustomSortComparer = SkyrimRecordComparers.ModeledComparer,
            },
            25);
    }
}
