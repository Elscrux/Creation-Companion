using CreationEditor.GUI.Models;
using Elscrux.WPF.Models;
using ReactiveUI;
namespace CreationEditor.GUI.Views.Controls; 

public class LoadingViewBase : ReactiveUserControl<NotificationItem> { }

public partial class LoadingView {
    public LoadingView() {
        InitializeComponent();
    }
}

