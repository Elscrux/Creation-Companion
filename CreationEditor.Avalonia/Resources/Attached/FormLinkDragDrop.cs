using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using CreationEditor.Avalonia.Behavior;
using CreationEditor.Avalonia.Constants;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Attached;

public sealed class FormLinkDragDrop : AvaloniaObject {
    private const string FormLink = "FormLink";

    private static readonly DragHandler DragHandler = new(DragStart);

    public static readonly AttachedProperty<bool> AllowDragDataGridProperty
        = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, bool>("AllowDragDataGrid");
    public static readonly AttachedProperty<bool> AllowDropDataGridProperty
        = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, bool>("AllowDropDataGrid");

    public static readonly AttachedProperty<Func<object?, IFormLinkIdentifier>> GetFormLinkProperty
        = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Func<object?, IFormLinkIdentifier>>("GetFormLink");
    public static readonly AttachedProperty<Action<IFormLinkIdentifier>> SetFormLinkProperty
        = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Action<IFormLinkIdentifier>>("SetFormLink");

    public static readonly AttachedProperty<Func<IFormLinkIdentifier, bool>> CanSetFormLinkProperty
        = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Func<IFormLinkIdentifier, bool>>("CanSetFormLink");

    public static bool GetAllowDragDataGrid(AvaloniaObject obj) => obj.GetValue(AllowDragDataGridProperty);
    public static void SetAllowDragDataGrid(AvaloniaObject obj, bool value) => obj.SetValue(AllowDragDataGridProperty, value);

    public static bool GetAllowDropDataGrid(AvaloniaObject obj) => obj.GetValue(AllowDropDataGridProperty);
    public static void SetAllowDropDataGrid(AvaloniaObject obj, bool value) => obj.SetValue(AllowDropDataGridProperty, value);

    public static Func<object?, IFormLinkIdentifier> GetGetFormLink(AvaloniaObject obj) => obj.GetValue(GetFormLinkProperty);
    public static void SetGetFormLink(AvaloniaObject obj, Func<object?, IFormLinkIdentifier> value) => obj.SetValue(GetFormLinkProperty, value);

    public static Action<IFormLinkIdentifier> GetSetFormLink(AvaloniaObject obj) => obj.GetValue(SetFormLinkProperty);
    public static void SetSetFormLink(AvaloniaObject obj, Action<IFormLinkIdentifier> value) => obj.SetValue(SetFormLinkProperty, value);

    public static Func<IFormLinkIdentifier, bool> GetCanSetFormLink(AvaloniaObject obj) => obj.GetValue(CanSetFormLinkProperty);
    public static void SetCanSetFormLink(AvaloniaObject obj, Func<IFormLinkIdentifier, bool> value) => obj.SetValue(CanSetFormLinkProperty, value);

    static FormLinkDragDrop() {
        AllowDragDataGridProperty.Changed
            .Subscribe(allowDragDataGrid => {
                var state = allowDragDataGrid.NewValue.GetValueOrDefault<bool>();

                switch (allowDragDataGrid.Sender) {
                    case DataGrid dataGrid: {
                        dataGrid.Unloaded -= OnDataGridUnloaded;
                        dataGrid.Unloaded += OnDataGridUnloaded;

                        dataGrid.LoadingRow -= DataGridOnLoadingRow;
                        dataGrid.LoadingRow += DataGridOnLoadingRow;

                        dataGrid.UnloadingRow -= OnDataGridOnUnloadingRow;
                        dataGrid.UnloadingRow += OnDataGridOnUnloadingRow;

                        break;

                        void DataGridOnLoadingRow(object? sender, DataGridRowEventArgs args) {
                            DragHandler.Unregister(args.Row, dataGrid);

                            if (state) {
                                DragHandler.Register(args.Row, dataGrid);
                            }
                        }

                        void OnDataGridOnUnloadingRow(object? sender, DataGridRowEventArgs args) => DragHandler.Unregister(args.Row, dataGrid);

                        void OnDataGridUnloaded(object? sender, RoutedEventArgs e) {
                            DragHandler.UnregisterIdentifier(dataGrid);
                            dataGrid.Unloaded -= OnDataGridUnloaded;
                        }
                    }
                    case Control control:
                        Toggle();

                        control.Loaded -= OnControlOnLoaded;
                        control.Loaded += OnControlOnLoaded;

                        control.Unloaded -= OnControlOnUnloaded;
                        control.Unloaded += OnControlOnUnloaded;
                        break;

                        void OnControlOnLoaded(object? o, RoutedEventArgs routedEventArgs) => Toggle();
                        void OnControlOnUnloaded(object? o, RoutedEventArgs routedEventArgs) => DragHandler.Unregister(control);

                        void Toggle() {
                            DragHandler.Unregister(control);

                            if (state) {
                                DragHandler.Register(control);
                            }
                        }
                }
            });

        AllowDropDataGridProperty.Changed
            .Subscribe(allowDropDataGrid => {
                var allowsDrop = allowDropDataGrid.NewValue.GetValueOrDefault<bool>();

                allowDropDataGrid.Sender.SetValue(DragDrop.AllowDropProperty, allowDropDataGrid.NewValue.Value);

                switch (allowDropDataGrid.Sender) {
                    case DataGrid dataGrid:
                        dataGrid.LoadingRow -= OnDataGridLoadedChanged;
                        dataGrid.LoadingRow += OnDataGridLoadedChanged;

                        dataGrid.UnloadingRow -= OnDataGridLoadedChanged;
                        dataGrid.UnloadingRow += OnDataGridLoadedChanged;
                        break;

                        void OnDataGridLoadedChanged(object? o, DataGridRowEventArgs args) => ToggleDataGrid(args);

                        void DropDataGrid(object? sender, DragEventArgs e) => Drop(dataGrid, e);

                        void ToggleDataGrid(DataGridRowEventArgs args) {
                            args.Row.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
                            args.Row.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
                            args.Row.RemoveHandler(DragDrop.DropEvent, DropDataGrid);

                            if (allowsDrop) {
                                args.Row.AddHandler(DragDrop.DragEnterEvent, DragEnter);
                                args.Row.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
                                args.Row.AddHandler(DragDrop.DropEvent, DropDataGrid);
                            }
                        }
                    case Control control:
                        ToggleControl();
                        if (!allowsDrop) return;

                        control.Loaded -= OnControlLoadedChanged;
                        control.Loaded += OnControlLoadedChanged;

                        control.Unloaded -= OnControlLoadedChanged;
                        control.Unloaded += OnControlLoadedChanged;
                        break;

                        void OnControlLoadedChanged(object? o, RoutedEventArgs routedEventArgs) => ToggleControl();
                        void ToggleControl() {
                            control.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
                            control.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
                            control.RemoveHandler(DragDrop.DropEvent, Drop);

                            if (allowsDrop) {
                                control.AddHandler(DragDrop.DragEnterEvent, DragEnter);
                                control.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
                                control.AddHandler(DragDrop.DropEvent, Drop);
                            }
                        }
                }
            });
    }

    public static Task DragStart(object? sender, object? identifier, PointerEventArgs e) {
        identifier ??= sender;
        if (identifier is not AvaloniaObject obj) return Task.CompletedTask;
        if (sender is not Visual pressed) return Task.CompletedTask;
        if (!e.GetCurrentPoint(pressed).Properties.IsLeftButtonPressed) return Task.CompletedTask;

        var formLinkGetter = GetGetFormLink(obj);
        var formLink = formLinkGetter(pressed.DataContext);

        var dataObject = new DataObject();
        dataObject.Set(FormLink, formLink);

        DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Copy);
        return Task.CompletedTask;
    }

    private static void Drop(object? sender, DragEventArgs e) {
        if (sender is not Visual visual) return;

        AdornerLayer.SetAdorner(visual, null);

        var formLink = GetData(e);
        if (formLink is null) return;

        var canSetFormLink = GetCanSetFormLink(visual);
        if (canSetFormLink is not null && !canSetFormLink(formLink)) return;

        var formLinkSetter = GetSetFormLink(visual);
        formLinkSetter(formLink);
    }

    private static void DragEnter(object? sender, DragEventArgs e) {
        if (sender is not Visual visual) return;

        var setFormLink = GetSetFormLink(visual);
        if (setFormLink is null) return;

        var formLink = GetData(e);
        if (formLink is null) return;

        var canSetFormLink = GetCanSetFormLink(visual);

        // Show adorner when target has setter for form link
        AdornerLayer.SetAdorner(
            visual,
            new Border {
                BorderBrush = canSetFormLink is not null && canSetFormLink(formLink) ? StandardBrushes.ValidBrush : StandardBrushes.InvalidBrush,
                BorderThickness = new Thickness(2),
                IsHitTestVisible = false,
            });
    }

    private static void DragLeave(object? sender, DragEventArgs e) {
        if (sender is not Visual visual) return;

        var setFormLink = GetSetFormLink(visual);
        if (setFormLink is null) return;

        // Hide adorner when target has setter for form link
        AdornerLayer.SetAdorner(visual, null);
    }

    private static IFormLinkIdentifier? GetData(DragEventArgs e) {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        if (e.Data?.Contains(FormLink) is not true) return null;

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var formLinkObject = e.Data?.Get(FormLink);

        return formLinkObject as IFormLinkGetter;
    }
}
