using Avalonia;
namespace CreationEditor.Avalonia.Attached;

public sealed class Annotations : AvaloniaObject {
    public static readonly AttachedProperty<string?> TitleProperty =
        AvaloniaProperty.RegisterAttached<Annotations, StyledElement, string?>("Title");

    public static string? GetTitle(AvaloniaObject element) => element.GetValue(TitleProperty);
    public static void SetTitle(AvaloniaObject element, string? titleValue) => element.SetValue(TitleProperty, titleValue);
}
