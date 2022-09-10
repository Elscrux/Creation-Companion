using CreationEditor.GUI.Models.Record.Browser;
using CreationEditor.GUI.Skyrim.Views.Record.List;
using CreationEditor.GUI.ViewModels.Record;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.References.ReferenceCache;
namespace CreationEditor.GUI.Skyrim.ViewModels.Record.List; 

public class SkyrimNpcListVM : RecordListVM<INpcGetter> {
    public SkyrimNpcListVM(IRecordBrowserSettings recordBrowserSettings, IReferenceQuery referenceQuery) : base(recordBrowserSettings, referenceQuery) {
        View = new SkyrimNpcList(this);
    }
}
