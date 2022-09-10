using CreationEditor.GUI.Skyrim.ViewModels.Record.List;
namespace CreationEditor.GUI.Skyrim.Views.Record.List; 

public partial class SkyrimNpcList {
    public SkyrimNpcList(SkyrimNpcListVM recordListVM) {
        InitializeComponent();

        DataContext = ViewModel = recordListVM;
    }
}
