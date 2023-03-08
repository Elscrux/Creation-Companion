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

    public void InsertColumn(int index, DataGridColumn column) {
        RecordGrid?.Columns.Insert(index, column);
    }

    public void AddColumn(DataGridColumn column) {
        RecordGrid?.Columns.Add(column);
    }

    public void ScrollToItem(object item) {
        if (RecordGrid == null) return;

        RecordGrid.SelectedItem = item;
        RecordGrid.ScrollIntoView(RecordGrid.SelectedItem, RecordGrid.Columns.First());
    }

    protected override void OnLoaded() {
        base.OnLoaded();

        RecordGrid.Columns.First().ClearSort();
        RecordGrid.Columns.First().Sort();
    }
}
