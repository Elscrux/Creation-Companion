using CreationEditor.GUI.ViewModels.Mod;
using ReactiveUI;
namespace CreationEditor.GUI.Views.Mod; 

public class ModDetailsViewBase : ReactiveUserControl<IModGetterVM> { }

public partial class ModDetails {
    public ModDetails() {
        InitializeComponent();
    }
}
