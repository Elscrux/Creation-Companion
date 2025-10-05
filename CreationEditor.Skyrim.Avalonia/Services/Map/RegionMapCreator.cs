using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Services.Map;

public sealed class RegionMapCreator(
    ILinkCacheProvider linkCacheProvider) {
    public Task<DrawingGroup> GetRegionMap(
        IWorldspaceGetter worldspace,
        int leftCoordinate = int.MinValue,
        int topCoordinate = int.MaxValue,
        int rightCoordinate = int.MaxValue,
        int bottomCoordinate = int.MinValue,
        int cellSize = 16,
        Func<ICellGetter, Color>? cellColorProvider = null) {
        cellColorProvider ??= DefaultCellColorProvider;

        if (leftCoordinate == int.MinValue) leftCoordinate = (int) worldspace.ObjectBoundsMin.X;
        if (rightCoordinate == int.MaxValue) rightCoordinate = (int) worldspace.ObjectBoundsMax.X;
        if (topCoordinate == int.MaxValue) topCoordinate = (int) worldspace.ObjectBoundsMax.Y;
        if (bottomCoordinate == int.MinValue) bottomCoordinate = (int) worldspace.ObjectBoundsMin.Y;

        return Task.Run(() => {
            var cellRectangles = worldspace.EnumerateCells()
                .Where(cell => cell.Grid is {} grid
                 && grid.Point.X >= leftCoordinate
                 && grid.Point.X <= rightCoordinate
                 && grid.Point.Y <= topCoordinate
                 && grid.Point.Y >= bottomCoordinate)
                .Select(cell => (Coordinates: WorldspaceToImageCoordinates(cell.Grid!.Point) * cellSize, Brush: cellColorProvider(cell)))
                .ToArray();

            return Dispatcher.UIThread.Invoke(() => {
                var drawingGroup = new DrawingGroup();
                foreach (var cellRectangle in cellRectangles) {
                    drawingGroup.Children.Add(new GeometryDrawing {
                        Brush = new SolidColorBrush(cellRectangle.Brush),
                        Geometry = new RectangleGeometry {
                            Rect = new Rect(
                                cellRectangle.Coordinates.X,
                                cellRectangle.Coordinates.Y,
                                cellSize,
                                cellSize)
                        }
                    });
                }

                return drawingGroup;
            });
        });
        
        P2Int WorldspaceToImageCoordinates(P2Int worldspacePosition) => new(
            (worldspacePosition.X - leftCoordinate),
            (topCoordinate - worldspacePosition.Y));
    }

    private Color DefaultCellColorProvider(ICellGetter cell) {
        if (cell.Regions is null) return Colors.Gray;

        var colors = cell.Regions.Select(regionLink => regionLink.TryResolve(linkCacheProvider.LinkCache))
            .WhereNotNull()
            .Select(region => region.MapColor)
            .WhereNotNull()
            .Select(colorValue => Color.FromArgb(colorValue.A,
                colorValue.R,
                colorValue.G,
                colorValue.B))
            .ToArray();

        if (colors.Length == 0) return Colors.Gray;

        return colors.Aggregate((a, b) => new Color(255,
            (byte) ((a.R + b.R) / 2),
            (byte) ((a.G + b.G) / 2),
            (byte) ((a.B + b.B) / 2)));
    }
}
