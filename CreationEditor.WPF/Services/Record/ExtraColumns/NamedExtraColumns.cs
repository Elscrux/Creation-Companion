using System.Windows.Controls;
using System.Windows.Data;
using Mutagen.Bethesda.Plugins.Aspects;
namespace CreationEditor.WPF.Services.Record;

public class NamedExtraColumns : ExtraColumns<INamed, INamedGetter> {
    public override IEnumerable<ExtraColumn> Columns {
        get {
            yield return new ExtraColumn(
                new DataGridTextColumn {
                    Header = "Name",
                    Binding = new Binding("Record.Name.String"),
                    CanUserSort = true,
                    Width = 100,
                },
                99);
        }
    }
}
