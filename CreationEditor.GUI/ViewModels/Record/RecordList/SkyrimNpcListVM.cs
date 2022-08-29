using CreationEditor.GUI.Views.Controls.Record.RecordList;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.GUI.ViewModels.Record.RecordList; 

public class SkyrimNpcListVM : RecordListVM<INpcGetter> {
    public SkyrimNpcListVM(ILinkCache scope) : base(scope) {
        View = new SkyrimNpcList(this);
    }
}
