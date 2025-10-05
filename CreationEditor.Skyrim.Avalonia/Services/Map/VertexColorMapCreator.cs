using Avalonia.Media;
using Avalonia.Media.Imaging;
using Mutagen.Bethesda.Skyrim;
using Size = Avalonia.Size;
namespace CreationEditor.Skyrim.Avalonia.Services.Map;

public sealed class VertexColorMapCreator {
    private readonly WorldMapCreator _creator = new(GetVertexColor);

    private static IEnumerable<ColorEntry> GetVertexColor(IWorldspaceGetter worldspace) {
        foreach (var cell in worldspace.EnumerateMajorRecords<ICellGetter>()) {
            if (cell.Landscape is null) continue;
            if (cell.Grid is null) continue;
            if (cell.Landscape.VertexColors is null) continue;

            var cellPosition = cell.Grid.Point * WorldMapCreator.LandscapePoints;
            foreach (var vertexColor in cell.Landscape.VertexColors) {
                if (vertexColor.Value is { X: 255, Y: 255, Z: 255 }) continue;

                yield return new ColorEntry(cellPosition + vertexColor.Key, Color.FromRgb(vertexColor.Value.X, vertexColor.Value.Y, vertexColor.Value.Z));
            }
        }
    }

    public Bitmap GetVertexColorMap(IWorldspaceGetter worldspace, Size imageSize, int leftCell, int rightCell, int topCell, int bottomCell) {
        return _creator.GetMap(worldspace, imageSize, leftCell, rightCell, topCell, bottomCell);
    }
}
