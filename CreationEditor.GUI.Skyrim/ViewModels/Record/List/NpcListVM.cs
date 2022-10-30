using System.Windows.Controls;
using System.Windows.Data;
using CreationEditor.Environment;
using CreationEditor.GUI.Models.Record.Browser;
using CreationEditor.GUI.Services.Record;
using CreationEditor.GUI.ViewModels.Record;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.References.ReferenceCache;
namespace CreationEditor.GUI.Skyrim.ViewModels.Record;

public class NpcListVM : RecordListVM<Npc, INpcGetter> {
    public NpcListVM(
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