using CreationEditor.GUI.Skyrim.ViewModels.Record;
namespace CreationEditor.GUI.Skyrim.Views.Record; 

public partial class NpcList {
    public NpcList(NpcListVM recordListVM) {
        InitializeComponent();

        DataContext = ViewModel = recordListVM;
    }
}
