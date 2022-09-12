using CreationEditor.Environment;
using CreationEditor.GUI.Models.Record.Browser;
using CreationEditor.GUI.Skyrim.Views.Record;
using CreationEditor.GUI.ViewModels.Record;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.References.ReferenceCache;
namespace CreationEditor.GUI.Skyrim.ViewModels.Record;

public class FactionListVM : RecordListVM<Faction, IFactionGetter> {
    public FactionListVM(
        IRecordBrowserSettings recordBrowserSettings,
        IReferenceQuery referenceQuery,
        IRecordEditorFactory recordEditorFactory,
        IRecordController recordController)
        : base(recordBrowserSettings, referenceQuery, recordEditorFactory, recordController) {
        View = new FactionList(this);
    }
}
