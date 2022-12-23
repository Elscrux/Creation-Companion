using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Record.List;
namespace CreationEditor.Avalonia.Views.Record; 

public partial class RecordList : ReactiveUserControl<IRecordListVM> {
    public RecordList() {
        InitializeComponent();
    }
    
    public void AddColumn(DataGridColumn column) {
        var dataGrid = this.FindControl<DataGrid>("PART_RecordGrid");

        dataGrid?.Columns.Add(column);
    }
}