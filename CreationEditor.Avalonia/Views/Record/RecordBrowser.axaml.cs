using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
namespace CreationEditor.Avalonia.Views.Record; 

public partial class RecordBrowser : ReactiveUserControl<IRecordBrowserVM> {
    public RecordBrowser() {
        InitializeComponent();
    }
    
    public RecordBrowser(IRecordBrowserVM recordBrowserVM) {
        InitializeComponent();

        DataContext = recordBrowserVM;
    }
}
