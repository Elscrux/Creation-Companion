using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Mod;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModView : ReactiveUserControl<IModGetterVM> {
    public ModView() {
        InitializeComponent();
    }
}
