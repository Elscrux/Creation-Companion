using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.DragAndDrop;
using CreationEditor.Avalonia.Constants;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Attached;

public class DragContext {
    public Dictionary<string, object?> Data { get; } = new();
}

public sealed class FormLinkDragDropHandler : DropHandlerBase, IDragHandler {
    public void BeforeDragDrop(object? sender, PointerEventArgs e, object? context) {
        if (context is not DragContext dragContext) return;
        if (!dragContext.Data.TryGetValue(DragDropExtended.DataSourceControl, out var source)) return;

        if (sender is not StyledElement senderElement) return;
        if (source is not AvaloniaObject sourceControl) return;

        var formLinkGetter = FormLinkDragDrop.GetGetFormLink(sourceControl);
        if (formLinkGetter is null) return;

        var formLink = formLinkGetter(senderElement.DataContext);

        dragContext.Data[FormLinkDragDrop.FormLinkData] = formLink;
    }

    public void AfterDragDrop(object? sender, PointerEventArgs e, object? context) {}

    public override void Enter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext) {
        base.Enter(sender, e, sourceContext, targetContext);

        if (sourceContext is not DragContext dragContext) return;
        if (!dragContext.Data.TryGetValue(FormLinkDragDrop.FormLinkData, out var link)) return;
        if (link is not IFormLinkIdentifier formLink) return;
        if (sender is not Visual visual) return;

        var setFormLink = FormLinkDragDrop.GetSetFormLink(visual);
        if (setFormLink is null) return;

        var canSetFormLink = FormLinkDragDrop.GetCanSetFormLink(visual);

        // Show adorner when target has setter for form link
        AdornerLayer.SetAdorner(
            visual,
            new Border {
                BorderBrush = canSetFormLink is not null && canSetFormLink(formLink)
                    ? StandardBrushes.HighlightBrush
                    : StandardBrushes.InvalidBrush,
                BorderThickness = new Thickness(2),
                IsHitTestVisible = false,
            });
    }

    public override void Leave(object? sender, RoutedEventArgs e) {
        base.Leave(sender, e);

        if (sender is not Visual visual) return;

        var setFormLink = FormLinkDragDrop.GetSetFormLink(visual);
        if (setFormLink is null) return;

        // Hide adorner when target has setter for form link
        AdornerLayer.SetAdorner(visual, null);
    }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state) {
        if (sourceContext is not DragContext dragContext) return false;
        if (!dragContext.Data.TryGetValue(FormLinkDragDrop.FormLinkData, out var link)) return false;
        if (link is not IFormLinkIdentifier formLink) return false;
        if (sender is not Visual visual) return false;

        var canSetFormLink = FormLinkDragDrop.GetCanSetFormLink(visual);
        return canSetFormLink is null || canSetFormLink(formLink);
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state) {
        if (sourceContext is not DragContext dragContext) return false;
        if (!dragContext.Data.TryGetValue(FormLinkDragDrop.FormLinkData, out var link)) return false;
        if (link is not IFormLinkIdentifier formLink) return false;
        if (sender is not Visual visual) return false;

        AdornerLayer.SetAdorner(visual, null);

        var canSetFormLink = FormLinkDragDrop.GetCanSetFormLink(visual);
        if (canSetFormLink is not null && !canSetFormLink(formLink)) return false;

        var formLinkSetter = FormLinkDragDrop.GetSetFormLink(visual);
        if (formLinkSetter is null) return false;

        formLinkSetter(formLink);

        return true;
    }
}

public sealed class FormLinkDragDrop : AvaloniaObject {
    public const string FormLinkData = "FormLink";

    public static readonly AttachedProperty<bool> AllowDragDataGridProperty
        = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, bool>("AllowDragDataGrid");
    public static readonly AttachedProperty<bool> AllowDropDataGridProperty
        = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, bool>("AllowDropDataGrid");

    public static readonly AttachedProperty<Func<object?, IFormLinkIdentifier>?> GetFormLinkProperty
        = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Func<object?, IFormLinkIdentifier>?>("GetFormLink");
    public static readonly AttachedProperty<Action<IFormLinkIdentifier>?> SetFormLinkProperty
        = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Action<IFormLinkIdentifier>?>("SetFormLink");

    public static readonly AttachedProperty<Func<IFormLinkIdentifier, bool>?> CanSetFormLinkProperty
        = AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Func<IFormLinkIdentifier, bool>?>("CanSetFormLink");

    public static Func<object?, IFormLinkIdentifier>? GetGetFormLink(AvaloniaObject obj) => obj.GetValue(GetFormLinkProperty);
    public static void SetGetFormLink(AvaloniaObject obj, Func<object?, IFormLinkIdentifier> value) => obj.SetValue(GetFormLinkProperty, value);

    public static Action<IFormLinkIdentifier>? GetSetFormLink(AvaloniaObject obj) => obj.GetValue(SetFormLinkProperty);
    public static void SetSetFormLink(AvaloniaObject obj, Action<IFormLinkIdentifier>? value) => obj.SetValue(SetFormLinkProperty, value);

    public static Func<IFormLinkIdentifier, bool>? GetCanSetFormLink(AvaloniaObject obj) => obj.GetValue(CanSetFormLinkProperty);
    public static void SetCanSetFormLink(AvaloniaObject obj, Func<IFormLinkIdentifier, bool>? value) => obj.SetValue(CanSetFormLinkProperty, value);
}
