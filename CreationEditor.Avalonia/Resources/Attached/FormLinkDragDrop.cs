using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using CreationEditor.Avalonia.Constants;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Attached;

public sealed class FormLinkDragDrop : AvaloniaObject {
    private const string FormLink = "FormLink";

    public static readonly AttachedProperty<bool> AllowDragDataGridProperty = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, bool>("AllowDragDataGrid");
    public static readonly AttachedProperty<bool> AllowDropDataGridProperty = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, bool>("AllowDropDataGrid");

    public static readonly AttachedProperty<Func<StyledElement, IFormLinkIdentifier>> GetFormLinkProperty = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Func<StyledElement, IFormLinkIdentifier>>("GetFormLink");
    public static readonly AttachedProperty<Action<IFormLinkIdentifier>> SetFormLinkProperty = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Action<IFormLinkIdentifier>>("SetFormLink");
    
    public static readonly AttachedProperty<Func<IFormLinkIdentifier, bool>> CanSetFormLinkProperty = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Func<IFormLinkIdentifier, bool>>("CanSetFormLink");

    public static bool GetAllowDragDataGrid(AvaloniaObject obj) => obj.GetValue(AllowDragDataGridProperty);
    public static void SetAllowDragDataGrid(AvaloniaObject obj, bool value) => obj.SetValue(AllowDragDataGridProperty, value);

    public static bool GetAllowDropDataGrid(AvaloniaObject obj) => obj.GetValue(AllowDropDataGridProperty);
    public static void SetAllowDropDataGrid(AvaloniaObject obj, bool value) => obj.SetValue(AllowDropDataGridProperty, value);

    public static Func<StyledElement, IFormLinkIdentifier> GetGetFormLink(AvaloniaObject obj) => obj.GetValue(GetFormLinkProperty);
    public static void SetGetFormLink(AvaloniaObject obj, Func<StyledElement, IFormLinkIdentifier> value) => obj.SetValue(GetFormLinkProperty, value);

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
                        dataGrid.LoadingRow -= DataGridOnLoadingRow;
                        dataGrid.LoadingRow += DataGridOnLoadingRow;
                        void DataGridOnLoadingRow(object? sender, DataGridRowEventArgs args) {
                            args.Row.RemoveHandler(InputElement.PointerPressedEvent, DragStartDataGrid);

                            if (state) {
                                // Tunnel routing is required because of the way data grid behaves - otherwise the event is not passed
                                args.Row.AddHandler(InputElement.PointerPressedEvent, DragStartDataGrid, RoutingStrategies.Tunnel);
                            }
                        }

                        dataGrid.UnloadingRow -= OnDataGridOnUnloadingRow;
                        dataGrid.UnloadingRow += OnDataGridOnUnloadingRow;
                        void OnDataGridOnUnloadingRow(object? sender, DataGridRowEventArgs args) {
                            args.Row.RemoveHandler(InputElement.PointerPressedEvent, DragStartDataGrid);
                        }

                        void DragStartDataGrid(object? sender, PointerPressedEventArgs e) => DragStart(dataGrid, sender, e);
                        break;
                    }
                    case Control control:
                        Toggle();

                        void Toggle() {
                            control.RemoveHandler(InputElement.PointerPressedEvent, DragStartControl);

                            if (state) {
                                control.AddHandler(InputElement.PointerPressedEvent, DragStartControl, RoutingStrategies.Tunnel);
                            }
                        }

                        control.Loaded -= OnControlOnLoaded;
                        control.Loaded += OnControlOnLoaded;
                        void OnControlOnLoaded(object? o, RoutedEventArgs routedEventArgs) => Toggle();

                        control.Unloaded -= OnControlOnUnloaded;
                        control.Unloaded += OnControlOnUnloaded;
                        void OnControlOnUnloaded(object? o, RoutedEventArgs routedEventArgs) => control.RemoveHandler(InputElement.PointerPressedEvent, DragStartControl);

                        void DragStartControl(object? sender, PointerPressedEventArgs e) => DragStart(sender, sender, e);
                        break;
                }
            });

        AllowDropDataGridProperty.Changed
            .Subscribe(allowDropDataGrid => {
                var state = allowDropDataGrid.NewValue.GetValueOrDefault<bool>();

                allowDropDataGrid.Sender.SetValue(DragDrop.AllowDropProperty, allowDropDataGrid.NewValue.Value);

                switch (allowDropDataGrid.Sender) {
                    case DataGrid dataGrid:
                        dataGrid.LoadingRow += (_, args) => {
                            args.Row.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
                            args.Row.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
                            args.Row.RemoveHandler(DragDrop.DropEvent, DropDataGrid);

                            if (state) {
                                args.Row.AddHandler(DragDrop.DragEnterEvent, DragEnter);
                                args.Row.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
                                args.Row.AddHandler(DragDrop.DropEvent, DropDataGrid);
                            }
                        };

                        dataGrid.UnloadingRow += (_, args) => {
                            args.Row.RemoveHandler(DragDrop.DropEvent, DropDataGrid);
                        };

                        void DropDataGrid(object? sender, DragEventArgs e) => Drop(dataGrid, e);
                        break;
                    case Control control:
                        void Toggle() {
                            control.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
                            control.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
                            control.RemoveHandler(DragDrop.DropEvent, Drop);

                            if (state) {
                                control.AddHandler(DragDrop.DragEnterEvent, DragEnter);
                                control.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
                                control.AddHandler(DragDrop.DropEvent, Drop);
                            }
                        }

                        control.Loaded -= OnControlOnLoaded;
                        control.Loaded += OnControlOnLoaded;
                        void OnControlOnLoaded(object? o, RoutedEventArgs routedEventArgs) => Toggle();

                        control.Unloaded -= OnControlOnUnloaded;
                        control.Unloaded += OnControlOnUnloaded;
                        void OnControlOnUnloaded(object? o, RoutedEventArgs routedEventArgs) => control.RemoveHandler(InputElement.PointerPressedEvent, DragStart);
                        break;
                }
            });
    }

    public static void DragStart(object? attachingObject, object? actualPressedSender, PointerPressedEventArgs e) {
        if (attachingObject is not AvaloniaObject obj) return;
        if (actualPressedSender is not Visual pressed) return;
        if (!e.GetCurrentPoint(pressed).Properties.IsLeftButtonPressed) return;

        var formLinkGetter = GetGetFormLink(obj);
        var formLink = formLinkGetter(pressed);

        var dataObject = new DataObject();
        dataObject.Set(FormLink, formLink);

        DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Copy);
    }

    private static void Drop(object? sender, DragEventArgs e) {
        if (sender is not Visual visual) return;

        AdornerLayer.SetAdorner(visual, null);

        var formLink = GetData(e);
        if (formLink == null) return;

        var canSetFormLink = GetCanSetFormLink(visual);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (canSetFormLink != null && !canSetFormLink(formLink)) return;

        var formLinkSetter = GetSetFormLink(visual);
        formLinkSetter(formLink);
    }

    private static void DragEnter(object? sender, DragEventArgs e) {
        if (sender is not Visual visual) return;

        var setFormLink = GetSetFormLink(visual);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (setFormLink == null) return;

        var formLink = GetData(e);
        if (formLink == null) return;

        var canSetFormLink = GetCanSetFormLink(visual);

        // Show adorner when target has setter for form link
        AdornerLayer.SetAdorner(visual, new Border {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            BorderBrush = canSetFormLink != null && canSetFormLink(formLink) ? StandardBrushes.ValidBrush : StandardBrushes.InvalidBrush,
            BorderThickness = new Thickness(2),
            IsHitTestVisible = false,
        });
    }

    private static void DragLeave(object? sender, DragEventArgs e) {
        if (sender is not Visual visual) return;

        var setFormLink = GetSetFormLink(visual);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (setFormLink == null) return;

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
