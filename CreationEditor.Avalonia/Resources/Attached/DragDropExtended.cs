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
using CreationEditor.Avalonia.Behavior;
using CreationEditor.Avalonia.Constants;
using Noggog;
namespace CreationEditor.Avalonia.Attached;

public sealed class DragDropExtended : AvaloniaObject {
    private const string DataGrid = "DataGrid";

    private static readonly DragHandler DragHandler = new(DragStart);

    public static readonly AttachedProperty<bool>
        AllowDragProperty = AvaloniaProperty.RegisterAttached<DragDropExtended, DataGrid, bool>("AllowDrag");
    public static readonly AttachedProperty<bool>
        AllowDropProperty = AvaloniaProperty.RegisterAttached<DragDropExtended, DataGrid, bool>("AllowDrop");
    public static readonly AttachedProperty<Func<object, bool>> CanDropProperty =
        AvaloniaProperty.RegisterAttached<DragDropExtended, DataGrid, Func<object, bool>>("CanDrop");
    public static readonly AttachedProperty<Func<object, object?>> DropSelectorProperty =
        AvaloniaProperty.RegisterAttached<DragDropExtended, DataGrid, Func<object, object?>>("DropSelector");

    public static bool GetAllowDrag(AvaloniaObject obj) => obj.GetValue(AllowDragProperty);
    public static void SetAllowDrag(AvaloniaObject obj, bool value) => obj.SetValue(AllowDragProperty, value);

    public static bool GetAllowDrop(AvaloniaObject obj) => obj.GetValue(AllowDropProperty);
    public static void SetAllowDrop(AvaloniaObject obj, bool value) => obj.SetValue(AllowDropProperty, value);

    public static Func<object, bool> GetCanDrop(AvaloniaObject obj) => obj.GetValue(CanDropProperty);
    public static void SetCanDrop(AvaloniaObject obj, Func<object, bool> value) => obj.SetValue(CanDropProperty, value);

    public static Func<object, object?> GetDropSelector(AvaloniaObject obj) => obj.GetValue(DropSelectorProperty);
    public static void SetDropSelector(AvaloniaObject obj, Func<object, object?> value) => obj.SetValue(DropSelectorProperty, value);

    private static Func<object, bool> GetCanDropOrDefault(DataGrid dataGrid) => GetCanDrop(dataGrid);
    private static Func<object, object?> GetDropSelectorOrDefault(DataGrid dataGrid) => GetDropSelector(dataGrid);

    static DragDropExtended() {
        AllowDragProperty.Changed
            .Subscribe(allowDrag => {
                if (allowDrag.Sender is not DataGrid dataGrid) return;

                var state = allowDrag.NewValue.GetValueOrDefault<bool>();

                dataGrid.Unloaded -= OnDataGridUnloaded;
                dataGrid.Unloaded += OnDataGridUnloaded;

                dataGrid.LoadingRow -= OnDataGridOnLoadingRow;
                dataGrid.LoadingRow += OnDataGridOnLoadingRow;

                dataGrid.UnloadingRow -= OnDataGridOnUnloadingRow;
                dataGrid.UnloadingRow += OnDataGridOnUnloadingRow;
                return;

                void OnDataGridOnLoadingRow(object? sender, DataGridRowEventArgs args) {
                    if (state) {
                        DragHandler.Unregister(args.Row);
                        DragHandler.Register(args.Row, dataGrid);
                    } else {
                        DragHandler.Unregister(args.Row, dataGrid);
                    }
                }

                void OnDataGridOnUnloadingRow(object? sender, DataGridRowEventArgs args) {
                    DragHandler.Unregister(args.Row, dataGrid);
                }

                void OnDataGridUnloaded(object? sender, RoutedEventArgs e) {
                    DragHandler.UnregisterIdentifier(dataGrid);
                    dataGrid.Unloaded -= OnDataGridUnloaded;
                }
            });

        AllowDropProperty.Changed
            .Subscribe(allowDrop => {
                if (allowDrop.Sender is not DataGrid dataGrid) return;

                dataGrid.SetValue(DragDrop.AllowDropProperty, allowDrop.NewValue.Value);
                dataGrid.RemoveHandler(Control.LoadedEvent, DataGridLoadedHandler);
                dataGrid.AddHandler(Control.LoadedEvent, DataGridLoadedHandler);

                var state = allowDrop.NewValue.GetValueOrDefault<bool>();

                dataGrid.LoadingRow -= OnDataGridOnLoadingRow;
                dataGrid.LoadingRow += OnDataGridOnLoadingRow;

                dataGrid.UnloadingRow -= OnDataGridOnUnloadingRow;
                dataGrid.UnloadingRow += OnDataGridOnUnloadingRow;
                return;

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

                void DataGridLoadedHandler(object? sender, RoutedEventArgs e) {
                    var border = dataGrid.FindDescendantOfType<Border>();
                    if (border is null) return;

                    border.RemoveHandler(DragDrop.DragEnterEvent, DataGridDragEnter);
                    border.AddHandler(DragDrop.DragEnterEvent, DataGridDragEnter);

                    border.RemoveHandler(DragDrop.DragLeaveEvent, DataGridDragLeave);
                    border.AddHandler(DragDrop.DragLeaveEvent, DataGridDragLeave);

                    border.RemoveHandler(DragDrop.DropEvent, DataGridDrop);
                    border.AddHandler(DragDrop.DropEvent, DataGridDrop);
                }

                void OnDataGridOnUnloadingRow(object? sender, DataGridRowEventArgs args) {
                    args.Row.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
                    args.Row.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
                    args.Row.RemoveHandler(DragDrop.DropEvent, Drop);
                }
            });
    }

    private static void DataGridDragEnter(object? sender, DragEventArgs e) {
        if (sender is not Border { Parent: DataGrid dataGrid }) return;

        var row = GetLastRow(dataGrid);
        if (row is null) return;

        // Get old data
        if (!GetData(
            row,
            e,
            out var oldDataGrid,
            out var oldList,
            out var newDataGrid,
            out var newList)) return;

        AdornerLayer.SetAdorner(
            row,
            new Rectangle {
                Fill = ReferenceEquals(oldList, newList) || CanDropAny(newDataGrid, oldDataGrid)
                    ? StandardBrushes.ValidBrush
                    : StandardBrushes.InvalidBrush,
                Height = 2,
                VerticalAlignment = VerticalAlignment.Bottom,
                IsHitTestVisible = false,
            });
    }

    private static void DataGridDragLeave(object? sender, DragEventArgs e) {
        if (sender is not Border { Parent: DataGrid dataGrid }) return;

        var row = GetLastRow(dataGrid);
        if (row is null) return;

        AdornerLayer.SetAdorner(row, null);
    }

    private static void DataGridDrop(object? sender, DragEventArgs e) {
        if (sender is not Border { Parent: DataGrid dataGrid }) return;

        var row = GetLastRow(dataGrid);
        if (row is null || !GetData(
            row,
            e,
            out var oldDataGrid,
            out var oldList,
            out var newDataGrid,
            out var newList)) return;

        AdornerLayer.SetAdorner(row, null);

        if (!ReferenceEquals(oldList, newList) && !CanDropAny(newDataGrid, oldDataGrid)) return;

        GetItems(oldDataGrid, newDataGrid, out var validItems, out var invalidItems);

        // Remove old items
        foreach (var selectedItem in validItems) {
            oldList.Remove(selectedItem);
        }

        // Add new items
        foreach (var selectedItem in validItems) {
            newList.Add(selectedItem);
        }

        // Set new selection
        SetNewSelection(newDataGrid, oldDataGrid, validItems, invalidItems);
    }

    private static Task DragStart(object? sender, object? identifier, PointerEventArgs e) {
        if (sender is not DataGridRow { DataContext: {} item } row) return Task.CompletedTask;
        if (!e.GetCurrentPoint(row).Properties.IsLeftButtonPressed) return Task.CompletedTask;

        // Only allow starting to drag when the source is a known background element
        if (e.Source is not StyledElement styledElement) return Task.CompletedTask;
        if (styledElement.Name is not "CellBorder" and not "BackgroundRectangle" and not "InvalidVisualElement") return Task.CompletedTask;

        var dataGrid = row.FindAncestorOfType<DataGrid>();
        if (dataGrid is null) return Task.CompletedTask;

        // Only start dragging when the row was previously selected
        if (!dataGrid.SelectedItems.Contains(item)) return Task.CompletedTask;

        var dataObject = new DataObject();
        dataObject.Set(DataGrid, dataGrid);
        DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Move);
        return Task.CompletedTask;
    }

    private static void DragEnter(object? sender, DragEventArgs e) {
        if (sender is not DataGridRow row) return;

        // Get old data
        if (!GetData(
            sender,
            e,
            out var oldDataGrid,
            out var oldList,
            out var newDataGrid,
            out var newList)) return;

        // Show adorner
        AdornerLayer.SetAdorner(
            row,
            new Rectangle {
                Fill = ReferenceEquals(oldList, newList) || CanDropAny(newDataGrid, oldDataGrid)
                    ? StandardBrushes.ValidBrush
                    : StandardBrushes.InvalidBrush,
                Height = 2,
                VerticalAlignment = VerticalAlignment.Top,
                IsHitTestVisible = false,
            });

        e.Handled = true;
    }

    private static void DragLeave(object? sender, DragEventArgs e) {
        if (sender is not DataGridRow row) return;

        // Hide adorner
        AdornerLayer.SetAdorner(row, null);
    }

    private static void Drop(object? sender, DragEventArgs e) {
        if (!GetData(
            sender,
            e,
            out var oldDataGrid,
            out var oldList,
            out var newDataGrid,
            out var newList)) return;

        if (sender is not DataGridRow { DataContext: not null } newRow) return;

        // Hide adorner
        AdornerLayer.SetAdorner(newRow, null);

        if (!ReferenceEquals(oldList, newList) && !CanDropAny(newDataGrid, oldDataGrid)) return;

        GetItems(oldDataGrid, newDataGrid, out var validItems, out var invalidItems);

        // Remove old items
        foreach (var selectedItem in validItems) {
            oldList.Remove(selectedItem);
        }

        // Add new items
        var newIndex = newRow.Index;
        if (newIndex > newList.Count) return;

        var offset = 0;
        foreach (var selectedItem in validItems) {
            newList.Insert(newIndex + offset, selectedItem);

            offset++;
        }

        // Set new selection
        SetNewSelection(newDataGrid, oldDataGrid, validItems, invalidItems);
    }
    private static bool GetData(
        object? sender,
        DragEventArgs e,
        [MaybeNullWhen(false)] out DataGrid outOldDataGrid,
        [MaybeNullWhen(false)] out IList outOldList,
        [MaybeNullWhen(false)] out DataGrid outNewDataGrid,
        [MaybeNullWhen(false)] out IList outNewList) {
        outOldDataGrid = null;
        outOldList = null;
        outNewDataGrid = null;
        outNewList = null;

        if (sender is not DataGridRow { DataContext: not null } newRow) return false;

        // Get old data
        if (!e.Data.Contains(DataGrid)) return false;

        var oldDataGridObject = e.Data.Get(DataGrid);
        if (oldDataGridObject is not DataGrid { ItemsSource: IList oldList } oldDataGrid) return false;

        // Get new data
        var newDataGrid = newRow.FindAncestorOfType<DataGrid>();
        if (newDataGrid?.ItemsSource is not IList newList) return false;

        outOldDataGrid = oldDataGrid;
        outOldList = oldList;
        outNewDataGrid = newDataGrid;
        outNewList = newList;

        return true;
    }

    private static bool CanDropAny(DataGrid newDataGrid, DataGrid oldDataGrid) {
        var canDrop = GetCanDropOrDefault(newDataGrid);
        var selector = GetDropSelectorOrDefault(newDataGrid);

        return oldDataGrid.SelectedItems
            .OfType<object>()
            .Select(selector)
            .NotNull()
            .Any(canDrop);
    }

    private static DataGridRow? GetLastRow(DataGrid dataGrid) {
        var presenter = dataGrid.FindDescendantOfType<DataGridRowsPresenter>();

        return presenter?.Children
            .OfType<DataGridRow>()
            .MaxBy(row => row.Index);
    }

    private static void GetItems(DataGrid oldDataGrid, DataGrid newDataGrid, out List<object> validItems, out List<object> invalidItems) {
        var canDrop = GetCanDropOrDefault(newDataGrid);
        var selector = GetDropSelectorOrDefault(newDataGrid);
        var sameGrid = ReferenceEquals(oldDataGrid, newDataGrid);

        validItems = oldDataGrid.SelectedItems
            .OfType<object>()
            .Select(selector)
            .NotNull()
            .Where(obj => sameGrid || canDrop(obj))
            .ToList();

        invalidItems = oldDataGrid.SelectedItems
            .OfType<object>()
            .Except(validItems)
            .ToList();
    }

    private static void SetNewSelection(
        DataGrid newDataGrid,
        DataGrid oldDataGrid,
        List<object> validItems,
        List<object> invalidItems) {
        newDataGrid.SelectedItems.Clear();
        foreach (var validItem in validItems) {
            newDataGrid.SelectedItems.Add(validItem);
        }

        oldDataGrid.SelectedItems.Clear();
        foreach (var invalidItem in invalidItems) {
            oldDataGrid.SelectedItems.Add(invalidItem);
        }
    }

}
