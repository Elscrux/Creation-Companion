using System.Reactive.Subjects;
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
using ReactiveUI;
namespace MapperPlugin.Views;

public partial class MapperView : ReactiveUserControl<MapperVM> {
    public MapperView() {
        InitializeComponent();
    }

    public MapperView(MapperVM vm) : this() {
        DataContext = vm;
    }

    public async Task ExportImage() {
        if (Map?.Source is null) return;

        var drawing = Drawings?.Source is DrawingImage dImage ? dImage.Drawing : null;
        var vertexColor = VertexColors?.Source as IImage;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        var includeMap = true;
        var includeMappings = drawing is not null;
        var includeVertexColor = vertexColor is not null;

        var mapImage = new ReplaySubject<IImage?>();
        mapImage.OnNext(Map.Source);
        var vertexColorsImage = new ReplaySubject<IImage?>();
        vertexColorsImage.OnNext(vertexColor);
        var drawingsImage = new ReplaySubject<IImage?>();
        drawingsImage.OnNext(Drawings?.Source);

        var dialog = new TaskDialog {
            Header = "Export Map",
            Content = new Grid {
                Children = {
                    new StackPanel {
                        Spacing = 5,
                        Children = {
                            new Grid {
                                Height = 450,
                                Width = Map.Source.Size.Width / Map.Source.Size.Height * 450,
                                Children = {
                                    new Image { [!Image.SourceProperty] = mapImage.ToBinding() },
                                    new Image { [!Image.SourceProperty] = vertexColorsImage.ToBinding() },
                                    new Image { [!Image.SourceProperty] = drawingsImage.ToBinding() },
                                }
                            },
                            new CheckBox {
                                Content = "Include Map",
                                Command = ReactiveCommand.Create(() => {
                                    includeMap = !includeMap;
                                    mapImage.OnNext(includeMap ? Map.Source : null);
                                }),
                                IsChecked = includeMap,
                            },
                            new CheckBox {
                                Content = "Include Mappings",
                                Command = ReactiveCommand.Create(() => {
                                    includeMappings = !includeMappings;
                                    drawingsImage.OnNext(includeMappings ? Drawings?.Source : null);
                                }),
                                IsChecked = includeMappings,
                                IsEnabled = includeMappings,
                            },
                            new CheckBox {
                                Content = "Include Vertex Color",
                                Command = ReactiveCommand.Create(() => {
                                    includeVertexColor = !includeVertexColor;
                                    vertexColorsImage.OnNext(includeVertexColor ? vertexColor : null);
                                }),
                                IsChecked = includeVertexColor,
                                IsEnabled = includeVertexColor,
                            },
                        }
                    }
                },
            },
            XamlRoot = this,
            Buttons = {
                new TaskDialogButton {
                    Text = "Export",
                    IsDefault = true,
                    DialogResult = TaskDialogStandardResult.OK,
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

        if (includeMap) Map.Source.Draw(drawingContext, bounds, bounds);
        if (includeVertexColor) vertexColor?.Draw(drawingContext, bounds, bounds);
        if (includeMappings) drawing?.Draw(drawingContext);

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
