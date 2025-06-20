using Avalonia;
using Avalonia.Input;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Attached.DragDrop;

public sealed class FormLinkDragDrop : AvaloniaObject, ICustomDragDropData<IFormLinkIdentifier> {

    public static readonly AttachedProperty<Func<object?, IFormLinkIdentifier>?> GetFormLinkProperty =
        AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Func<object?, IFormLinkIdentifier>?>("GetFormLink");
    public static readonly AttachedProperty<Action<IFormLinkIdentifier>?> SetFormLinkProperty =
        AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Action<IFormLinkIdentifier>?>("SetFormLink");

    public static readonly AttachedProperty<Func<IFormLinkIdentifier, bool>?> CanSetFormLinkProperty =
        AvaloniaProperty.RegisterAttached<FormLinkDragDrop, InputElement, Func<IFormLinkIdentifier, bool>?>("CanSetFormLink");

    public static Func<object?, IFormLinkIdentifier>? GetGetFormLink(AvaloniaObject obj) => obj.GetValue(GetFormLinkProperty);
    public static void SetGetFormLink(AvaloniaObject obj, Func<object?, IFormLinkIdentifier> value) => obj.SetValue(GetFormLinkProperty, value);

    public static Action<IFormLinkIdentifier>? GetSetFormLink(AvaloniaObject obj) => obj.GetValue(SetFormLinkProperty);
    public static void SetSetFormLink(AvaloniaObject obj, Action<IFormLinkIdentifier>? value) => obj.SetValue(SetFormLinkProperty, value);

    public static Func<IFormLinkIdentifier, bool>? GetCanSetFormLink(AvaloniaObject obj) => obj.GetValue(CanSetFormLinkProperty);
    public static void SetCanSetFormLink(AvaloniaObject obj, Func<IFormLinkIdentifier, bool>? value) => obj.SetValue(CanSetFormLinkProperty, value);

    public static string Data => "FormLink";
    public static Func<object?, IFormLinkIdentifier>? GetData(AvaloniaObject obj) => GetGetFormLink(obj);
    public static Action<IFormLinkIdentifier>? GetSetData(AvaloniaObject obj) => GetSetFormLink(obj);
    public static Func<IFormLinkIdentifier, bool>? GetCanSetData(AvaloniaObject obj) => GetCanSetFormLink(obj);
}
