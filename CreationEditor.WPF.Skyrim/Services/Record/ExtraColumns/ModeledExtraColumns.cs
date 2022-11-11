using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
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
                Width = 200,
            }, 25);
        }
    }
}
