using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.Constants;
namespace CreationEditor.Avalonia.Attached; 

public sealed class DragDropExtended : AvaloniaObject {
    private const string DataGridRow = "DataGridRow";
    private const string DataGrid = "DataGrid";
    
    public static readonly AttachedProperty<bool> AllowDragProperty = AvaloniaProperty.RegisterAttached<DragDropExtended, DataGrid, bool>("AllowDrag");
    public static readonly AttachedProperty<bool> AllowDropProperty = AvaloniaProperty.RegisterAttached<DragDropExtended, DataGrid, bool>("AllowDrop");
    public static readonly AttachedProperty<Func<object, bool>> CanDropProperty = AvaloniaProperty.RegisterAttached<DragDropExtended, DataGrid, Func<object, bool>>("CanDrop");
    public static readonly AttachedProperty<Func<object, object?>> DropSelectorProperty = AvaloniaProperty.RegisterAttached<DragDropExtended, DataGrid, Func<object, object?>>("DropSelector");
    
    public static bool GetAllowDrag(AvaloniaObject obj) => obj.GetValue(AllowDragProperty);
    public static void SetAllowDrag(AvaloniaObject obj, bool value) => obj.SetValue(AllowDragProperty, value);
    
    public static bool GetAllowDrop(AvaloniaObject obj) => obj.GetValue(AllowDropProperty);
    public static void SetAllowDrop(AvaloniaObject obj, bool value) => obj.SetValue(AllowDropProperty, value);
    
    public static Func<object, bool> GetCanDrop(AvaloniaObject obj) => obj.GetValue(CanDropProperty);
    public static void SetCanDrop(AvaloniaObject obj, Func<object, bool> value) => obj.SetValue(CanDropProperty, value);
    
    public static Func<object, object?> GetDropSelector(AvaloniaObject obj) => obj.GetValue(DropSelectorProperty);
    public static void SetDropSelector(AvaloniaObject obj, Func<object, object?> value) => obj.SetValue(DropSelectorProperty, value);
    
    static DragDropExtended() {
        AllowDragProperty.Changed
            .Subscribe(allowDrag => {
                if (allowDrag.Sender is not DataGrid dataGrid) return;
                
                var state = allowDrag.NewValue.GetValueOrDefault<bool>();
                
                dataGrid.LoadingRow -= OnDataGridOnLoadingRow;
                dataGrid.LoadingRow += OnDataGridOnLoadingRow;
                void OnDataGridOnLoadingRow(object? sender, DataGridRowEventArgs args) {
                    if (state) {
                        // Tunnel routing is required because of the way data grid behaves - otherwise the event is not passed 
                        args.Row.RemoveHandler(InputElement.PointerPressedEvent, DragStart);
                        args.Row.AddHandler(InputElement.PointerPressedEvent, DragStart, RoutingStrategies.Tunnel);
                    } else {
                        args.Row.RemoveHandler(InputElement.PointerPressedEvent, DragStart);
                    }
                }
                
                dataGrid.UnloadingRow -= OnDataGridOnUnloadingRow;
                dataGrid.UnloadingRow += OnDataGridOnUnloadingRow;
                void OnDataGridOnUnloadingRow(object? sender, DataGridRowEventArgs args) {
                    args.Row.RemoveHandler(InputElement.PointerPressedEvent, DragStart);
                }
            });
        
        AllowDropProperty.Changed
            .Subscribe(allowDrop => {
                if (allowDrop.Sender is not DataGrid dataGrid) return;
                
                dataGrid.SetValue(DragDrop.AllowDropProperty, allowDrop.NewValue.Value);
                
                var state = allowDrop.NewValue.GetValueOrDefault<bool>();
                
                dataGrid.LoadingRow -= OnDataGridOnLoadingRow;
                dataGrid.LoadingRow += OnDataGridOnLoadingRow;
                void OnDataGridOnLoadingRow(object? sender, DataGridRowEventArgs args) {
                    if (state) {
                        args.Row.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
                        args.Row.AddHandler(DragDrop.DragEnterEvent, DragEnter);
                        
                        args.Row.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
                        args.Row.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
                        
                        args.Row.RemoveHandler(DragDrop.DropEvent, Drop);
                        args.Row.AddHandler(DragDrop.DropEvent, Drop);
                    } else {
                        args.Row.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
                        args.Row.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
                        args.Row.RemoveHandler(DragDrop.DropEvent, Drop);
                    }
                }
                
                dataGrid.UnloadingRow -= OnDataGridOnUnloadingRow;
                dataGrid.UnloadingRow += OnDataGridOnUnloadingRow;
                void OnDataGridOnUnloadingRow(object? sender, DataGridRowEventArgs args) {
                    args.Row.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
                    args.Row.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
                    args.Row.RemoveHandler(DragDrop.DropEvent, Drop);
                }
            });
    }
    
    private static void DragStart(object? sender, PointerPressedEventArgs e) {
        if (sender is not DataGridRow row) return;
        if (!e.GetCurrentPoint(row).Properties.IsLeftButtonPressed) return;
        
        // Only allow starting to drag when the source is directly under the data grid cell
        if (e.Source is not StyledElement styledElement) return;
        if (styledElement.Parent is not DataGridCell
         && styledElement.TemplatedParent is not DataGridCell) return;
        
        var dataGrid = row.FindAncestorOfType<DataGrid>();
        if (dataGrid == null) return;
        
        var dataObject = new DataObject();
        dataObject.Set(DataGridRow, row);
        dataObject.Set(DataGrid, dataGrid);
        DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Move);
    }
    
    private static void DragEnter(object? sender, DragEventArgs e) {
        if (sender is not DataGridRow { DataContext: {} hoverItem } row) return;
        
        // Get old data
        if (!GetData(sender, e,
            out _, out var dragItem,
            out _, out var oldList, 
            out var newDataGrid, out var newList)) return;

        var canDrop = GetCanDrop(newDataGrid);
        
        // Show adorner
        AdornerLayer.SetAdorner(row, new Rectangle {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            Fill = ReferenceEquals(oldList, newList) || canDrop == null || canDrop(dragItem) ? StandardBrushes.ValidBrush : StandardBrushes.InvalidBrush,
            Height = 2,
            VerticalAlignment = VerticalAlignment.Top,
            IsHitTestVisible = false,
        });
    }
    
    private static void DragLeave(object? sender, DragEventArgs e) {
        if (sender is not DataGridRow row) return;
        
        // Hide adorner
        AdornerLayer.SetAdorner(row, null);
    }
    
    private static void Drop(object? sender, DragEventArgs e) {
        if (!GetData(sender, e,
            out var oldRow, out var dragItem,
            out var oldDataGrid, out var oldList, 
            out var newDataGrid, out var newList)) return;
        
        if (sender is not DataGridRow { DataContext: {} } newRow) return;
        
        // Hide adorner
        AdornerLayer.SetAdorner(newRow, null);

        var canDrop = GetCanDrop(newDataGrid);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (!ReferenceEquals(oldList, newList) && canDrop != null && !canDrop(dragItem)) return;
        
        // Get indices
        var oldIndex = oldRow.GetIndex();
        var newIndex = newRow.GetIndex();
        if (newIndex > newList.Count) return;
        
        var sameGrid = ReferenceEquals(oldDataGrid, newDataGrid);
        if (sameGrid && oldIndex == newIndex) return;
        
        // Remove old item
        oldDataGrid.SelectedItem = null;
        oldList.Remove(dragItem);
        
        // Add new item
        newList.Insert(sameGrid && oldIndex < newIndex ? newIndex - 1 : newIndex, dragItem);
        
        // Select new item
        newDataGrid.SelectedIndex = newIndex;
        if (!sameGrid) oldDataGrid.SelectedItem = null;
    }

    private static bool GetData(
        object sender,
        DragEventArgs e,
        [MaybeNullWhen(false)] out DataGridRow outOldRow,
        [MaybeNullWhen(false)] out object outDragItem,
        [MaybeNullWhen(false)] out DataGrid outOldDataGrid,
        [MaybeNullWhen(false)] out IList outOldList,
        [MaybeNullWhen(false)] out DataGrid outNewDataGrid,
        [MaybeNullWhen(false)] out IList outNewList) {
        outOldRow = null;
        outDragItem = null;
        outOldDataGrid = null;
        outOldList = null;
        outNewDataGrid = null;
        outNewList = null;

        if (sender is not DataGridRow { DataContext: {} hoverItem } newRow) return false;

        // Get old data
        if (e.Data?.Contains(DataGridRow) is not true) return false;

        var oldRowObject = e.Data?.Get(DataGridRow);
        if (oldRowObject is not DataGridRow { DataContext: {} dragItem } oldRow) return false;

        if (e.Data?.Contains(DataGrid) is not true) return false;

        var oldDataGridObject = e.Data?.Get(DataGrid);
        if (oldDataGridObject is not DataGrid { Items: IList oldList } oldDataGrid) return false;

        // Get new data
        var newDataGrid = newRow.FindAncestorOfType<DataGrid>();
        if (newDataGrid?.Items is not IList newList) return false;

        var selector = newDataGrid.GetValue(DropSelectorProperty);
        
        var hoverType = hoverItem.GetType();
        
        outDragItem = dragItem;
        if (dragItem.GetType() != hoverType) {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (selector == null) return false;

            var transformedDragItem = selector(dragItem);
            if (transformedDragItem?.GetType() != hoverType) return false;

            outDragItem = transformedDragItem;
        }
        
        outOldRow = oldRow;
        outOldDataGrid = oldDataGrid;
        outOldList = oldList;
        outNewDataGrid = newDataGrid;
        outNewList = newList;

        return true;
    }
}
