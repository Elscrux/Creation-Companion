using Avalonia.Controls;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Record.List;
namespace CreationEditor.Avalonia.Views.Record; 

public partial class RecordList : ReactiveUserControl<IRecordListVM> {
    public RecordList() {
        InitializeComponent();
    }
    
    public RecordList(IEnumerable<DataGridColumn> columns) : this() {
        foreach (var column in columns) {
            AddColumn(column);
        }
    }
    
    public void AddColumn(DataGridColumn column) {
        RecordGrid?.Columns.Add(column);
    }
}