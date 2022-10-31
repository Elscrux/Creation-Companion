using System.Windows.Controls;
using System.Windows.Data;
using CreationEditor.Environment;
using CreationEditor.WPF.Models.Record.Browser;
using CreationEditor.WPF.Services.Record;
using CreationEditor.WPF.ViewModels.Record;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.References.ReferenceCache;
namespace CreationEditor.WPF.Skyrim.ViewModels.Record;

public class FactionListVM : RecordListVM<Faction, IFactionGetter> {
    public FactionListVM(
        IReferenceQuery referenceQuery,
        IRecordBrowserSettings recordBrowserSettings,
        IRecordEditorController recordEditorControllerController,
        IRecordController recordController)
        : base(referenceQuery, recordBrowserSettings, recordEditorControllerController, recordController) {
        ExtraColumns.Add(new DataGridTextColumn {
            Header = "Name",
            Binding = new Binding("Record.Name.String"),
            CanUserSort = true,
        });
    }
}