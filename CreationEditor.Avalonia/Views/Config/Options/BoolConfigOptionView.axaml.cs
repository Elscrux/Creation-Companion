using CreationEditor.Avalonia.ViewModels.Config.Options;
using ReactiveUI.Avalonia;
namespace CreationEditor.Avalonia.Views.Config.Options;

public partial class BoolConfigOptionView : ReactiveUserControl<BoolConfigOptionVM> {
    public BoolConfigOptionView() {
        InitializeComponent();
    }
}
