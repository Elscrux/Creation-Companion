using CreationEditor.GUI.ViewModels.Record.RecordList;
using ReactiveUI;
namespace CreationEditor.GUI.Views.Controls.Record.RecordList; 

public class GenericRecordListViewBase : ReactiveUserControl<IRecordListVM> { }

public partial class GenericRecordList {
    public GenericRecordList(IRecordListVM recordListVM) {
        InitializeComponent();

        DataContext = ViewModel = recordListVM;
    }
}

public class GenericRecordList<TRecordListVM> : ReactiveUserControl<IRecordListVM> 
    where TRecordListVM : IRecordListVM {
    public GenericRecordList(TRecordListVM recordListVM) {
        DataContext = ViewModel = recordListVM;
    }
}
