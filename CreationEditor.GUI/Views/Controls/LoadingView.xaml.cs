using CreationEditor.GUI.Models;
using ReactiveUI;
namespace CreationEditor.GUI.Views.Controls; 

public class LoadingViewBase : ReactiveUserControl<LoadingItem> { }

public partial class LoadingView {
    public LoadingView() {
        InitializeComponent();
    }
}

