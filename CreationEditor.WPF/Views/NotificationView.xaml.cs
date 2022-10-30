using Elscrux.WPF.Models;
using ReactiveUI;
namespace CreationEditor.GUI.Views; 

public class NotificationViewBase : ReactiveUserControl<NotificationItem> { }

public partial class NotificationView {
    public NotificationView() {
        InitializeComponent();
    }
}

