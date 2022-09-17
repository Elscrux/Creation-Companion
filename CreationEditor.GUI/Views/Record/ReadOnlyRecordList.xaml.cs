using CreationEditor.GUI.ViewModels.Record;
using ReactiveUI;
namespace CreationEditor.GUI.Views.Record; 

public class RecordListViewBase : ReactiveUserControl<IRecordListVM> { }

public partial class ReadOnlyRecordList {
    public ReadOnlyRecordList(IRecordListVM recordListVM) {
        InitializeComponent();

        DataContext = ViewModel = recordListVM;
    }
}
