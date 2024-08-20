using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Skyrim.Avalonia.Resources.Comparer;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.List.ExtraColumns;

public class PlacedExtraColumns : ExtraColumns<IPlacedGetter> {
    public override IEnumerable<ExtraColumn> CreateColumns() {
        yield return new ExtraColumn(new DataGridTextColumn {
                Header = "Base",
                Binding = new Binding("Base.EditorID", BindingMode.OneWay),
                CanUserSort = true,
                CustomSortComparer = SkyrimRecordComparers.BaseComparer,
                Width = new DataGridLength(200),
            },
            250);
    }
}
