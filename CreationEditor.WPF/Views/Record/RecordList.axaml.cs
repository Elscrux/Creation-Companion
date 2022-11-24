using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CreationEditor.WPF.ViewModels.Record;

namespace CreationEditor.WPF.Views.Record; 

public partial class RecordList : ReactiveUserControl<IRecordListVM> {
    public RecordList() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
    
    public void AddColumn(DataGridColumn column) {
        var dataGrid = this.FindControl<DataGrid>("PART_RecordGrid");

        dataGrid?.Columns.Add(column);
    }
}