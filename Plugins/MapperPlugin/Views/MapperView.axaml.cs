using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.Constants;
using FluentAvalonia.UI.Controls;
using MapperPlugin.ViewModels;
using Mutagen.Bethesda;
namespace MapperPlugin.Views;

public partial class MapperView : ReactiveUserControl<MapperVM> {
    public MapperView() {
        InitializeComponent();
    }

    public MapperView(MapperVM vm) : this() {
        DataContext = vm;
    }

    public async Task ExportImage() {
        if (Map?.Source is null || Drawings?.Source is not DrawingImage { Drawing: {} drawing }) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        var dialog = new TaskDialog {
            Header = "Export Map",
            Content = new Grid {
                Children = {
                    new Image {
                        Source = Drawings.Source,
                    },
                },
            },
            XamlRoot = this,
            Buttons = {
                new TaskDialogButton {
                    Text = "Export All",
                    IsDefault = true,
                    DialogResult = TaskDialogStandardResult.OK,
                },
                new TaskDialogButton {
                    Text = "Export Mask",
                    DialogResult = TaskDialogStandardResult.Yes,
                },
                TaskDialogButton.CancelButton,
            },
        };

        var exportMode = await dialog.ShowAsync();
        if (exportMode is TaskDialogStandardResult.Cancel) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
            Title = "Export image",
            SuggestedFileName = "heatmap-export",
            FileTypeChoices = StandardFileTypes.ImageAll,
            ShowOverwritePrompt = true,
        });
        if (file is null) return;

        // Prepare the render target
        var drawingsSize = Map.Source.Size;
        var size = new PixelSize((int) drawingsSize.Width, (int) drawingsSize.Height);
        var bounds = new Rect(size.ToSize(1));
        var renderTargetBitmap = new RenderTargetBitmap(size);
        var drawingContext = renderTargetBitmap.CreateDrawingContext();

        // Also draw the map if selected
        if (exportMode is TaskDialogStandardResult.OK) {
            Map.Source.Draw(drawingContext, bounds, bounds);
        }

        // Draw the drawings
        drawing.Draw(drawingContext);

        renderTargetBitmap.Save(file.Path.LocalPath);
    }

    private void Drawings_OnPointerPressed(object? sender, PointerPressedEventArgs e) {
        if (ViewModel?.ImageSource is null) return;
        if (!e.GetCurrentPoint(Drawings).Properties.IsLeftButtonPressed) return;

        var position = e.GetPosition(Drawings);
        var imageSize = ViewModel.ImageSource.Size;
        var drawingsBounds = Drawings.Bounds;
        var imageCoordinates = new Point(
            position.X / drawingsBounds.Width * imageSize.Width,
            position.Y / drawingsBounds.Height * imageSize.Height);

        var record = ViewModel.HeatmapCreator.GetMappingAt(imageCoordinates);
        if (record is null) return;

        if (!ViewModel.LinkCacheProvider.LinkCache.TryResolveIdentifier(record.ToStandardizedIdentifier(), out var editorId)) return;

        var flyout = new Flyout {
            Content = new TextBlock {
                Text = editorId is null
                    ? record.FormKey.ToString()
                    : editorId + " " + record.FormKey,
            },
        };

        flyout.ShowAt(Drawings, true);
    }
}
