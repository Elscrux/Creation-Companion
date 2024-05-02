using System.Diagnostics;
using Avalonia;
using Avalonia.Media;
using CreationEditor.Avalonia;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim;
using MapperPlugin.ViewModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Size = Avalonia.Size;
namespace MapperPlugin.Services;

using RecordSpots = (IFormLinkIdentifier Record, Dictionary<Point, int> Spots);
using MappingSpots = Dictionary<IFormLinkIdentifier, Dictionary<Point, int>>;

public sealed class HeatmapCreator {
    private const int CellSize = 4096;
    private const int Clustering = CellSize / 4;

    private Dictionary<MarkingMapping, MappingSpots>? _spotsPerMapping;

    private readonly MappingSpots _spotsCache = new();

    public DrawingImage? GetDrawing(Size imageSize, int markingSize, int leftCell, int rightCell, int topCell, int bottomCell) {
        if (_spotsPerMapping is null) return null;

        var (imageWidth, imageHeight) = imageSize;
        var mainGroup = GetDrawingGroup(imageWidth, imageHeight);

        var worldspaceWidth = (rightCell - leftCell) * CellSize;
        var worldspaceHeight = (bottomCell - topCell) * CellSize;
        var leftCoordinate = leftCell * CellSize;
        var topCoordinate = topCell * CellSize;
        var rightCoordinate = rightCell * CellSize;
        var bottomCoordinate = bottomCell * CellSize;

        var widthScale = worldspaceWidth / imageWidth;
        var heightScale = worldspaceHeight / imageHeight;

        foreach (var (mapping, mappingSpots) in _spotsPerMapping) {
            if (!mapping.Enable) continue;

            foreach (var (record, spots) in mappingSpots) {
                foreach (var (worldspacePosition, count) in spots) {
                    // Skip points outside worldspace bounds
                    if (worldspacePosition.X < leftCoordinate || worldspacePosition.X > rightCoordinate) continue;
                    if (worldspacePosition.Y < bottomCoordinate || worldspacePosition.Y > topCoordinate) continue;

                    var size = markingSize * mapping.Size * (1 + Math.Log(count) / 2);
                    // Convert worldspace position to image position
                    // Worldspace is centered at 0,0
                    // Image has top-left corner at 0,0
                    var imagePosition = new Point(
                        (worldspacePosition.X - leftCoordinate) / widthScale,
                        (worldspacePosition.Y - topCoordinate) / heightScale);

                    var color = mapping is { UseQuery: true, UseRandomColorsInQuery: true }
                        ? ColorExtension.RandomColorRgb()
                        : mapping.Color;

                    var geometryDrawing = new GeometryDrawing {
                        Brush = new SolidColorBrush(color),
                        Geometry = new EllipseGeometry {
                            Center = imagePosition,
                            RadiusX = size,
                            RadiusY = size,
                        },
                    };

                    mainGroup.Children.Add(geometryDrawing);
                }
            }
        }

        return new DrawingImage(mainGroup);
    }

    public async Task CalculateSpots(IEnumerable<MarkingMapping> mappings, ILinkCache linkCache, IRecordReferenceController recordReferenceController, FormKey worldspace) {
        var spotsPerMapping = new Dictionary<MarkingMapping, MappingSpots>();

        foreach (var mapping in mappings.ToList()) {
            var spotsPerRecord = new MappingSpots();
            spotsPerMapping.Add(mapping, spotsPerRecord);

            var allRecordSpots = await Task.WhenAll(mapping.CurrentRecords
                .Select(record => Task.Run(() => GetSpots(record))));

            foreach (var (record, spots) in allRecordSpots) {
                spotsPerRecord.Add(record, spots);
            }
        }

        _spotsPerMapping = spotsPerMapping;

        RecordSpots GetSpots(IFormLinkIdentifier record) {
            if (_spotsCache.TryGetValue(record, out var cachedSpots)) return (record, cachedSpots);

            var timestamp = Stopwatch.GetTimestamp();
            var spots = new Dictionary<Point, int>();
            foreach (var reference in recordReferenceController.GetReferences(record.FormKey)) {
                if (!reference.Type.InheritsFrom(typeof(IPlacedGetter))) continue;
                if (!linkCache.TryResolveSimpleContext(reference, out var referenceRecordContext)) continue;
                if (referenceRecordContext.Record is not IPlacedGetter { Placement: not null } placed) continue;
                if (referenceRecordContext.Parent?.Record is not ICellGetter parentCell) continue;

                var position = placed.Placement.Position;
                Point point;
                if ((parentCell.Flags & Cell.Flag.IsInteriorCell) == 0) {
                    // Exterior cell - make sure it's in the worldspace
                    if (!referenceRecordContext.TryGetParent<IWorldspaceGetter>(out var parentWorldspace)) continue;
                    if (parentWorldspace.FormKey != worldspace) continue;

                    point = Cluster(position);
                } else {
                    // Interior cell - find first exit to worldspace
                    var doorInWorldspace = parentCell.GetDoorsToWorldspace(linkCache).FirstOrDefault();
                    if (doorInWorldspace?.Placement is null) continue;

                    point = Cluster(doorInWorldspace.Placement.Position);
                }

                if (spots.TryGetValue(point, out var count)) {
                    spots[point] = count + 1;
                } else {
                    spots.Add(point, 1);
                }
            }
            Console.WriteLine($"GetPoints for {record.FormKey}: {Stopwatch.GetElapsedTime(timestamp, Stopwatch.GetTimestamp())}");

            _spotsCache.Add(record, spots);
            return (record, spots);

            Point Cluster(P3Float position) {
                return new Point(
                    position.X - (position.X % Clustering),
                    position.Y - (position.Y % Clustering));
            }
        }

    }

    private static DrawingGroup GetDrawingGroup(double width, double height) {
        var mainGroup = new DrawingGroup();
        mainGroup.Children.Add(new GeometryDrawing {
            Geometry = new LineGeometry(new Point(0, 0), new Point(0, height)),
            Pen = new Pen(Brushes.Transparent, 100)
        });
        mainGroup.Children.Add(new GeometryDrawing {
            Geometry = new LineGeometry(new Point(width, 0), new Point(width, height)),
            Pen = new Pen(Brushes.Transparent, 100)
        });
        mainGroup.Children.Add(new GeometryDrawing {
            Geometry = new LineGeometry(new Point(0, 0), new Point(width, 0)),
            Pen = new Pen(Brushes.Transparent, 100)
        });
        mainGroup.Children.Add(new GeometryDrawing {
            Geometry = new LineGeometry(new Point(0, height), new Point(width, height)),
            Pen = new Pen(Brushes.Transparent, 100)
        });
        return mainGroup;
    }


}
