using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
namespace CreationEditor.Avalonia.Attached; 

public sealed class DragDropExtended : AvaloniaObject {
    private const string DataGridRow = "DataGridRow";
    private const string DataGrid = "DataGrid";
    
    public static readonly AttachedProperty<bool> AllowDragProperty = AvaloniaProperty.RegisterAttached<DragDropExtended, DataGrid, bool>("AllowDrag");
    public static readonly AttachedProperty<bool> AllowDropProperty = AvaloniaProperty.RegisterAttached<DragDropExtended, DataGrid, bool>("AllowDrop");
    
    public static bool GetAllowDrag(AvaloniaObject obj) => obj.GetValue(AllowDragProperty);
    public static void SetAllowDrag(AvaloniaObject obj, bool value) => obj.SetValue(AllowDragProperty, value);
    
    public static bool GetAllowDrop(AvaloniaObject obj) => obj.GetValue(AllowDropProperty);
    public static void SetAllowDrop(AvaloniaObject obj, bool value) => obj.SetValue(AllowDropProperty, value);
    
    public static IBrush? Brush => 
        Application.Current != null
     && Application.Current.TryFindResource("SystemAccentColor", out var obj)
     && obj is Color color 
            ? new SolidColorBrush(color)
            : null;
    
    static DragDropExtended() {
        AllowDragProperty.Changed
            .Subscribe(allowDrag => {
                if (allowDrag.Sender is not DataGrid dataGrid) return;
                
                var state = allowDrag.NewValue.GetValueOrDefault<bool>();
                dataGrid.LoadingRow += (_, args) => {
                    if (state) {
                        // Tunnel routing is required because of the way data grid behaves - otherwise the event is not passed 
                        args.Row.RemoveHandler(InputElement.PointerPressedEvent, DragStart);
                        args.Row.AddHandler(InputElement.PointerPressedEvent, DragStart, RoutingStrategies.Tunnel);
                    } else {
                        args.Row.RemoveHandler(InputElement.PointerPressedEvent, DragStart);
                    }
                };
                
                dataGrid.UnloadingRow += (_, args) => {
                    args.Row.RemoveHandler(InputElement.PointerPressedEvent, DragStart);
                };
            });
        
        AllowDropProperty.Changed
            .Subscribe(allowDrop => {
                if (allowDrop.Sender is not DataGrid dataGrid) return;
                
                dataGrid.SetValue(DragDrop.AllowDropProperty, allowDrop.NewValue.Value);
                
                var state = allowDrop.NewValue.GetValueOrDefault<bool>();
                dataGrid.LoadingRow += (_, args) => {
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
                };
                
                dataGrid.UnloadingRow += (_, args) => {
                    args.Row.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
                    args.Row.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
                    args.Row.RemoveHandler(DragDrop.DropEvent, Drop);
                };
            });
    }
    
    private static void DragStart(object? sender, PointerPressedEventArgs e) {
        if (sender is not DataGridRow row) return;
        
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
        
        // Only allow if type matches
        if (e.Data?.Contains(DataGridRow) is not true) return;
        var oldRowObject = e.Data?.Get(DataGridRow);
        if (oldRowObject is not DataGridRow { DataContext: {} dragItem }) return;
        if (dragItem.GetType() != hoverItem.GetType()) return;
        
        // Show adorner
        AdornerLayer.SetAdorner(row, new Rectangle {
            Fill = Brush,
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
        if (sender is not DataGridRow { DataContext: {} hoverItem } newRow) return;
        
        // Hide adorner
        AdornerLayer.SetAdorner(newRow, null);
        
        // Get old data
        if (e.Data?.Contains(DataGridRow) is not true) return;
        var oldRowObject = e.Data?.Get(DataGridRow);
        if (oldRowObject is not DataGridRow { DataContext: {} dragItem } oldRow) return;
        if (dragItem.GetType() != hoverItem.GetType()) return;
        
        if (e.Data?.Contains(DataGrid) is not true) return;
        var oldDataGridObject = e.Data?.Get(DataGrid);
        if (oldDataGridObject is not DataGrid { Items: IList oldList } oldDataGrid) return;
        
        // Get new data
        var newDataGrid = newRow.FindAncestorOfType<DataGrid>();
        if (newDataGrid?.Items is not IList newList) return;
        
        // Get indices
        var oldIndex = oldRow.GetIndex();
        var newIndex = newRow.GetIndex();
        if (newIndex > newList.Count) return;
        
        var sameGrid = ReferenceEquals(oldDataGrid, newDataGrid);
        if (sameGrid && ReferenceEquals(dragItem, hoverItem)) return;
        
        // Remove old item
        oldList.Remove(dragItem);
        
        // Add new item
        newList.Insert(sameGrid && oldIndex < newIndex ? newIndex - 1 : newIndex, dragItem);
        
        // Select new item
        newDataGrid.SelectedIndex = newIndex;
        if (!sameGrid) oldDataGrid.SelectedItem = null;
    }
}
