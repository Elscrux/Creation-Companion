using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.DragAndDrop;
namespace CreationEditor.Avalonia.Attached;

public sealed class DragDropExtended : AvaloniaObject {
    public const string DataSourceControl = "DragDropExtended_SourceList";
    public const string DataStartPosition = "DragDropExtended_StartPosition";
    public const string DataCurrentIndex = "DragDropExtended_Index";

    public static readonly AttachedProperty<bool>
        AllowDragProperty = AvaloniaProperty.RegisterAttached<DragDropExtended, Interactive, bool>("AllowDrag");
    public static readonly AttachedProperty<bool>
        AllowDropProperty = AvaloniaProperty.RegisterAttached<DragDropExtended, Interactive, bool>("AllowDrop");
    public static readonly AttachedProperty<Func<object, bool>> CanDropProperty =
        AvaloniaProperty.RegisterAttached<DragDropExtended, Interactive, Func<object, bool>>("CanDrop", (_ => true));
    public static readonly AttachedProperty<Func<object, object?>> DropSelectorProperty =
        AvaloniaProperty.RegisterAttached<DragDropExtended, Interactive, Func<object, object?>>("DropSelector", (o => o));
    public static readonly AttachedProperty<IDragHandler?> DragHandlerProperty =
        AvaloniaProperty.RegisterAttached<DragDropExtended, Interactive, IDragHandler?>("DragHandler");

    public static bool GetAllowDrag(AvaloniaObject obj) => obj.GetValue(AllowDragProperty);
    public static void SetAllowDrag(AvaloniaObject obj, bool value) => obj.SetValue(AllowDragProperty, value);

    public static bool GetAllowDrop(AvaloniaObject obj) => obj.GetValue(AllowDropProperty);
    public static void SetAllowDrop(AvaloniaObject obj, bool value) => obj.SetValue(AllowDropProperty, value);

    public static Func<object, bool> GetCanDrop(AvaloniaObject obj) => obj.GetValue(CanDropProperty);
    public static void SetCanDrop(AvaloniaObject obj, Func<object, bool> value) => obj.SetValue(CanDropProperty, value);

    public static Func<object, object?> GetDropSelector(AvaloniaObject obj) => obj.GetValue(DropSelectorProperty);
    public static void SetDropSelector(AvaloniaObject obj, Func<object, object?> value) => obj.SetValue(DropSelectorProperty, value);

    public static IDragHandler? GetDragHandler(AvaloniaObject obj) => obj.GetValue(DragHandlerProperty);
    public static void SetDragHandler(AvaloniaObject obj, IDragHandler? value) => obj.SetValue(DragHandlerProperty, value);
}
