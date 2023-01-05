using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Logging;
namespace CreationEditor.Avalonia.Views.Logging; 

public partial class LogView : ReactiveUserControl<ILogVM> {
    public LogView() {
        InitializeComponent();
    }
    
    public LogView(ILogVM logVM) : this() {
        DataContext = logVM;
    }
}
