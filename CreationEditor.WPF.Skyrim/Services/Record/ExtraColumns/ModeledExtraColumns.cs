using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.WPF.Services.Record;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.WPF.Skyrim.Services.Record.ExtraColumns;

public class ModeledExtraColumns : ExtraColumns<IModeled, IModeledGetter> {
    public override IEnumerable<ExtraColumn> Columns {
        get {
            yield return new ExtraColumn(new DataGridTextColumn {
                Header = "Model",
                Binding = new Binding("Record.Model.File.DataRelativePath"),
                CanUserSort = true,
                Width = new DataGridLength(200),
            }, 25);
        }
    }
}
