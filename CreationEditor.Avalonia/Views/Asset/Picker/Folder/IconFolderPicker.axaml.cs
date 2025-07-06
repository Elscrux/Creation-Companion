using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using CreationEditor.Avalonia.Constants;
namespace CreationEditor.Avalonia.Views.Asset.Picker.Folder;

[TemplatePart(DropButton, typeof(Button))]
public class IconFolderPicker : AFolderPicker {
    public const string DropButton = "PART_DropButton";

    public static readonly StyledProperty<bool> ShowIconProperty
        = AvaloniaProperty.Register<IconFolderPicker, bool>(nameof(ShowIcon), true);

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

        FolderPath = e.Data.Get("Folder") as string;
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
