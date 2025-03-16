using Avalonia.Media.Imaging;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Size = Avalonia.Size;
namespace MapperPlugin.Services;

public readonly record struct ColorEntry(P2Int Position, P3UInt8 Color);

public sealed class VertexColorMapCreator {
    private readonly WorldMapCreator _creator = new(GetVertexColor);

    private static IEnumerable<ColorEntry> GetVertexColor(IWorldspaceGetter worldspace) {
        foreach (var cell in worldspace.EnumerateMajorRecords<ICellGetter>()) {
            if (cell.Landscape is null) continue;
            if (cell.Grid is null) continue;
            if (cell.Landscape.VertexColors is null) continue;

            var cellPosition = cell.Grid.Point * WorldMapCreator.LandscapePoints;
            foreach (var vertexColor in cell.Landscape.VertexColors) {
                yield return new ColorEntry(cellPosition + vertexColor.Key, vertexColor.Value);
            }
        }
    }

    public Bitmap GetVertexColorMap(IWorldspaceGetter worldspace, Size imageSize, int leftCell, int rightCell, int topCell, int bottomCell) {
        return _creator.GetMap(worldspace, imageSize, leftCell, rightCell, topCell, bottomCell);
    }
}
