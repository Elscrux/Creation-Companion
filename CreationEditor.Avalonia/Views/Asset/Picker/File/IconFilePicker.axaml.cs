using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using CreationEditor.Avalonia.Constants;
using Path = System.IO.Path;
namespace CreationEditor.Avalonia.Views.Asset.Picker.File;

[TemplatePart(DropButton, typeof(Button))]
public class IconFilePicker : AFilePicker {
    public const string DropButton = "PART_DropButton";

    public static readonly StyledProperty<bool> ShowIconProperty
        = AvaloniaProperty.Register<IconFilePicker, bool>(nameof(ShowIcon));

    public bool ShowIcon {
        get => GetValue(ShowIconProperty);
        set => SetValue(ShowIconProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        var button = e.NameScope.Get<Button>(DropButton);

        button.RemoveHandler(DragDrop.DropEvent, Drop);
        button.AddHandler(DragDrop.DropEvent, Drop);

        button.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
        button.AddHandler(DragDrop.DragEnterEvent, DragEnter);

        button.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
        button.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
    }

    private void Drop(object? sender, DragEventArgs e) {
        RemoveAdorner(sender);

        var imageAllPatterns = FilePickerFileTypes.ImageAll.Patterns?.Select(x => x.TrimStart('*')).ToArray();
        if (imageAllPatterns is null) return;

        var filePath = e.Data
            .GetFiles()?
            .FirstOrDefault(file => imageAllPatterns.Contains(Path.GetExtension(file.Name)));

        if (filePath is null) return;

        FilePath = filePath.Path.AbsolutePath;
    }

    private static void DragEnter(object? sender, DragEventArgs e) {
        if (sender is Visual visual) {
            AdornerLayer.SetAdorner(visual,
                new Rectangle {
                    Fill = StandardBrushes.HighlightBrush,
                    Opacity = 0.5,
                    IsHitTestVisible = false,
                });
        }
    }

    private static void DragLeave(object? sender, DragEventArgs e) {
        RemoveAdorner(sender);
    }

    private static void RemoveAdorner(object? sender) {
        if (sender is Visual visual) {
            AdornerLayer.SetAdorner(visual, null);
        }
    }
}
