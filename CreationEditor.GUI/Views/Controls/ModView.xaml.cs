using CreationEditor.GUI.ViewModels;
using ReactiveUI;
namespace CreationEditor.GUI.Views.Controls; 

public class ModDetailsViewBase : ReactiveUserControl<IModGetterVM> { }

public partial class ModDetails {
    public ModDetails() {
        InitializeComponent();
    }
}
