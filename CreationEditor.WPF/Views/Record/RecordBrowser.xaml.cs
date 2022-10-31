using CreationEditor.WPF.ViewModels.Record;
using ReactiveUI;
namespace CreationEditor.WPF.Views.Record; 

public class RecordBrowserViewBase : ReactiveUserControl<IRecordBrowserVM> { }

public partial class RecordBrowser {
    public RecordBrowser(IRecordBrowserVM recordBrowserVM) {
        InitializeComponent();

        DataContext = ViewModel = recordBrowserVM;
    }
}
