using CreationEditor.GUI.ViewModels.Record;
using ReactiveUI;
namespace CreationEditor.GUI.Views.Controls.Record; 

public class RecordBrowserViewBase : ReactiveUserControl<IRecordBrowserVM> { }

public partial class RecordBrowser {
    public RecordBrowser(IRecordBrowserVM recordBrowserVM) {
        InitializeComponent();

        DataContext = ViewModel = recordBrowserVM;
    }
}
