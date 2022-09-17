using CreationEditor.GUI.ViewModels.Record;
using Syncfusion.UI.Xaml.Grid;
namespace CreationEditor.GUI.Views.Record; 

public partial class RecordList {
    protected readonly SfDataGrid RecordGrid;
    
    public RecordList(RecordListVM recordListVM) {
        InitializeComponent();

        RecordGrid = RecordGridControl;

        DataContext = recordListVM;
    }
}

