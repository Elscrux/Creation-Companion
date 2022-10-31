using CreationEditor.WPF.ViewModels.Mod;
using ReactiveUI;
namespace CreationEditor.WPF.Views.Mod; 

public class ModDetailsViewBase : ReactiveUserControl<IModGetterVM> { }

public partial class ModDetails {
    public ModDetails() {
        InitializeComponent();
    }
}
