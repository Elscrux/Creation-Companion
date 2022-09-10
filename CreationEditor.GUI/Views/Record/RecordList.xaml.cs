using CreationEditor.GUI.ViewModels.Record;
using ReactiveUI;
namespace CreationEditor.GUI.Views.Record; 

public class RecordListViewBase : ReactiveUserControl<IRecordListVM> { }

public partial class RecordList {
    public RecordList(IRecordListVM recordListVM) {
        InitializeComponent();

        DataContext = ViewModel = recordListVM;
    }
}
