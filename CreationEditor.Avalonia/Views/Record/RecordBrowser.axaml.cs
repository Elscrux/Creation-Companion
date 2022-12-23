using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
namespace CreationEditor.Avalonia.Views.Record; 

public partial class RecordBrowser : UserControl {
    public RecordBrowser() {
        InitializeComponent();
    }
    
    public RecordBrowser(IRecordBrowserVM recordBrowserVM) {
        InitializeComponent();

        DataContext = recordBrowserVM;
    }
}
