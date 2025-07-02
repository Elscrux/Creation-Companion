using Avalonia;
using CreationEditor.Avalonia.Views.Asset.Picker.File;
namespace CreationEditor.Avalonia.Views.Asset.Picker.Folder;

public class TextFolderPicker : AFolderPicker {
    public static readonly StyledProperty<string?> WatermarkProperty
        = AvaloniaProperty.Register<TextFilePicker, string?>(nameof(Watermark));

    public string? Watermark {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    public static readonly StyledProperty<bool> AllowTextEditProperty
        = AvaloniaProperty.Register<TextFilePicker, bool>(nameof(AllowTextEdit));

    public bool AllowTextEdit {
        get => GetValue(AllowTextEditProperty);
        set => SetValue(AllowTextEditProperty, value);
    }
}
