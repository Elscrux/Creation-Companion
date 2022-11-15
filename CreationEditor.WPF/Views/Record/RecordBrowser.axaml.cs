using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CreationEditor.WPF.ViewModels.Record;
namespace CreationEditor.WPF.Views.Record; 

public partial class RecordBrowser : UserControl {
    public RecordBrowser() {
        InitializeComponent();
    }
    
    public RecordBrowser(IRecordBrowserVM recordBrowserVM) {
        InitializeComponent();

        DataContext = recordBrowserVM;
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
