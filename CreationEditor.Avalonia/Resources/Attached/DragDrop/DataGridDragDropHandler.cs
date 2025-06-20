using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
namespace CreationEditor.Avalonia.Attached.DragDrop;

public sealed class DataGridDragDropHandler : DragDropHandler<DataGrid, DataGridRow> {
    protected override IEnumerable GetItems(DataGrid control) => control.ItemsSource;
    protected override IList GetSelectedItems(DataGrid control) => control.SelectedItems;
    protected override int IndexOfElement(DataGrid control, DataGridRow element) => element.Index;
    protected override IEnumerable<DataGridRow> GetElements(DataGrid control) {
        var presenter = control.FindDescendantOfType<DataGridRowsPresenter>();
        if (presenter == null) return [];

        return presenter.Children.OfType<DataGridRow>();
    }
    protected override DataGridRow? GetElementAt(DataGrid control, int index) {
        var presenter = control.FindDescendantOfType<DataGridRowsPresenter>();

        return presenter?.Children
            .OfType<DataGridRow>()
            .FirstOrDefault(row => row.Index == index);
    }
    protected override DataGridRow? GetElement(DataGrid control, object? item) {
        var presenter = control.FindDescendantOfType<DataGridRowsPresenter>();

        return presenter?.Children
            .OfType<DataGridRow>()
            .FirstOrDefault(row => row.DataContext == item);
    }
    protected override DataGridRow? GetLastElement(DataGrid control) {
        var presenter = control.FindDescendantOfType<DataGridRowsPresenter>();

        return presenter?.Children
            .OfType<DataGridRow>()
            .MaxBy(row => row.Index);
    }
}
