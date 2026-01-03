using System.Collections.Concurrent;
using Avalonia;
using Avalonia.Media;
using CreationEditor.Avalonia;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Mutagen.References;
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

public sealed class HeatmapCreator(IModService modService) {
    private const int CellSize = 4096;
    private const int Clustering = CellSize / 4;

    private Dictionary<MarkingMapping, MappingSpots>? _spotsPerMapping;

    private readonly ConcurrentDictionary<FormKey, Dictionary<Point, int>> _spotsCache = new();

    private int _leftCoordinate;
    private int _topCoordinate;
    private double _widthImageScale;
    private double _heightImageScale;
    private int _markingSize;

    public DrawingImage? GetDrawing(Size imageSize, int markingSize, int leftCell, int rightCell, int topCell, int bottomCell) {
        if (_spotsPerMapping is null) return null;

        var (imageWidth, imageHeight) = imageSize;
        var mainGroup = GetDrawingGroup(imageWidth, imageHeight);

        _markingSize = markingSize;
        var worldspaceWidth = Math.Abs(rightCell - leftCell) * CellSize;
        var worldspaceHeight = Math.Abs(topCell - bottomCell) * CellSize;
        _leftCoordinate = leftCell * CellSize;
        _topCoordinate = topCell * CellSize;
        var rightCoordinate = rightCell * CellSize;
        var bottomCoordinate = bottomCell * CellSize;

        _widthImageScale = worldspaceWidth / imageWidth;
        _heightImageScale = worldspaceHeight / imageHeight;

        foreach (var (mapping, mappingSpots) in _spotsPerMapping) {
            if (!mapping.Enable) continue;

            foreach (var (_, spots) in mappingSpots) {
                var color = mapping is { UseQuery: true, UseRandomColorsInQuery: true }
                    ? ColorExtension.RandomColorRgb()
                    : mapping.Color;

                foreach (var (worldspacePosition, _) in spots) {
                    // Skip points outside worldspace bounds
                    if (worldspacePosition.X < _leftCoordinate || worldspacePosition.X > rightCoordinate) continue;
                    if (worldspacePosition.Y < bottomCoordinate || worldspacePosition.Y > _topCoordinate) continue;

                    var size = GetSize(mapping.Size);
                    // Convert worldspace position to image position
                    // Worldspace is centered at 0,0
                    // Image has top-left corner at 0,0
                    var imagePosition = WorldspaceToImageCoordinates(worldspacePosition);

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

    public async Task CalculateSpots(
        IEnumerable<MarkingMapping> mappings,
        ILinkCache linkCache,
        IReferenceService referenceService,
        FormKey worldspace) {
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
            if (_spotsCache.TryGetValue(record.FormKey, out var cachedSpots)) return (record, cachedSpots);

            var spots = new Dictionary<Point, int>();
            foreach (var reference in referenceService.GetRecordReferences(record)) {
                if (!reference.Type.InheritsFrom(typeof(IPlacedGetter))) continue;

                // If the reference's mod does not have access to the worldspace's mod, skip it
                if (!modService.IsModOrHasMasterTransitive(reference.FormKey.ModKey, worldspace.ModKey)) continue;

                if (!linkCache.TryResolveSimpleContext(reference, out var referenceRecordContext)) continue;
                if (referenceRecordContext.Record is not IPlacedGetter { IsDeleted: false, Placement: not null } placed) continue;
                if (referenceRecordContext.Parent?.Record is not ICellGetter parentCell) continue;

                var position = placed.Placement.Position;
                Point point;
                if (parentCell.Flags.HasFlag(Cell.Flag.IsInteriorCell)) {
                    // Interior cell - find first exit to worldspace
                    var doorInWorldspace = parentCell.GetDoorsToWorldspace(linkCache).FirstOrDefault();
                    if (doorInWorldspace?.Placement is null) continue;

                    point = Cluster(doorInWorldspace.Placement.Position);
                } else {
                    // Exterior cell - make sure it's in the worldspace
                    if (!referenceRecordContext.TryGetParent<IWorldspaceGetter>(out var parentWorldspace)) continue;
                    if (parentWorldspace.FormKey != worldspace) continue;

                    point = Cluster(position);
                }

                if (spots.TryGetValue(point, out var count)) {
                    spots[point] = count + 1;
                } else {
                    spots.Add(point, 1);
                }
            }

            _spotsCache.TryAdd(record.FormKey, spots);
            return (record, spots);

            Point Cluster(P3Float position) {
                return new Point(
                    position.X - position.X % Clustering,
                    position.Y - position.Y % Clustering);
            }
        }

    }

    private static DrawingGroup GetDrawingGroup(double width, double height) {
        var mainGroup = new DrawingGroup();
        const int thickness = 10;
        const int halfThickness = thickness / 2;
        mainGroup.Children.Add(new GeometryDrawing {
            Geometry = new LineGeometry(new Point(halfThickness, halfThickness), new Point(halfThickness, height - halfThickness)),
            Pen = new Pen(Brushes.Transparent, thickness),
        });
        mainGroup.Children.Add(new GeometryDrawing {
            Geometry = new LineGeometry(new Point(width - halfThickness, halfThickness), new Point(width - halfThickness, height - halfThickness)),
            Pen = new Pen(Brushes.Transparent, thickness),
        });
        mainGroup.Children.Add(new GeometryDrawing {
            Geometry = new LineGeometry(new Point(halfThickness, halfThickness), new Point(width - halfThickness, halfThickness)),
            Pen = new Pen(Brushes.Transparent, thickness),
        });
        mainGroup.Children.Add(new GeometryDrawing {
            Geometry = new LineGeometry(new Point(halfThickness, height - halfThickness), new Point(width - halfThickness, height - halfThickness)),
            Pen = new Pen(Brushes.Transparent, thickness),
        });
        return mainGroup;
    }

    public IFormLinkIdentifier? GetMappingAt(Point imageCoordinates) {
        if (_spotsPerMapping is null) return null;

        foreach (var (mapping, mappingSpots) in _spotsPerMapping) {
            var imageSize = GetSize(mapping.Size);

            foreach (var (record, spots) in mappingSpots) {
                foreach (var (worldspacePosition, _) in spots) {
                    var (x, y) = WorldspaceToImageCoordinates(worldspacePosition);

                    var xDist = Math.Abs(x - imageCoordinates.X);
                    var yDist = Math.Abs(y - imageCoordinates.Y);

                    if (xDist < imageSize && yDist < imageSize) {
                        return record;
                    }
                }
            }
        }

        return null;
    }

    private Point WorldspaceToImageCoordinates(Point worldspacePosition) => new(
        (worldspacePosition.X - _leftCoordinate) / _widthImageScale,
        (_topCoordinate - worldspacePosition.Y) / _heightImageScale);

    private double GetSize(float mappingSize) => _markingSize * mappingSize * (1 + Math.Log(1) / 2);

    public void ClearCache() => _spotsCache.Clear();
}
