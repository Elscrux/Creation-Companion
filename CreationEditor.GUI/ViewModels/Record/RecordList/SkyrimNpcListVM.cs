using CreationEditor.GUI.Models.Record.RecordBrowser;
using CreationEditor.GUI.Views.Controls.Record.RecordList;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.References.ReferenceCache;
namespace CreationEditor.GUI.ViewModels.Record.RecordList; 

public class SkyrimNpcListVM : RecordListVM<INpcGetter> {
    public SkyrimNpcListVM(IRecordBrowserSettings recordBrowserSettings, IReferenceQuery referenceQuery) : base(recordBrowserSettings, referenceQuery) {
        View = new SkyrimNpcList(this);
    }
}
