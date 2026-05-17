using Avalonia;
namespace CreationEditor.Avalonia.Views.Asset.Picker.File;

public class TextFilePicker : AFilePicker {
    public static readonly StyledProperty<string?> PlaceholderTextProperty
        = AvaloniaProperty.Register<TextFilePicker, string?>(nameof(PlaceholderText));

    public string? PlaceholderText {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly StyledProperty<bool> AllowTextEditProperty
        = AvaloniaProperty.Register<TextFilePicker, bool>(nameof(AllowTextEdit));

    public bool AllowTextEdit {
        get => GetValue(AllowTextEditProperty);
        set => SetValue(AllowTextEditProperty, value);
    }
}
