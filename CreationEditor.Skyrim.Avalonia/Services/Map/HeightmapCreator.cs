using Avalonia.Media;
using Avalonia.Media.Imaging;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Size = Avalonia.Size;
namespace CreationEditor.Skyrim.Avalonia.Services.Map;

public sealed class HeightmapCreator {
    public readonly record struct HeightEntry(P2Int Position, float Height);

    private readonly WorldMapCreator _creator = new(GetHeightColors);

    private static IEnumerable<HeightEntry> GetHeightValues(IWorldspaceGetter worldspace) {
        var heightValues = new List<HeightEntry>(
            (int) (worldspace.ObjectBoundsMax.Y - worldspace.ObjectBoundsMin.Y
              * (int) (worldspace.ObjectBoundsMax.X - worldspace.ObjectBoundsMin.X)
              * WorldMapCreator.LandscapePoints * WorldMapCreator.LandscapePoints));

        foreach (var cell in worldspace.EnumerateMajorRecords<ICellGetter>()) {
            if (cell.Landscape is null) continue;
            if (cell.Grid is null) continue;
            if (cell.Landscape.VertexHeightMap is null) continue;

            var cellPosition = cell.Grid.Point * WorldMapCreator.LandscapePoints;
            foreach (var height in cell.Landscape.VertexHeightMap.HeightMap) {
                var heightValue = cell.Landscape.VertexHeightMap.Offset + height.Value * 8;
                heightValues.Add(new HeightEntry(cellPosition + height.Key, heightValue));
            }
        }

        return heightValues;
    }

    private static IEnumerable<ColorEntry> GetHeightColors(IWorldspaceGetter worldspace) {
        var heightValues = GetHeightValues(worldspace).ToArray();

        var minHeight = heightValues[0].Height;
        var maxHeight = heightValues[0].Height;

        foreach (var entry in heightValues) {
            minHeight = Math.Min(minHeight, entry.Height);
            maxHeight = Math.Max(maxHeight, entry.Height);
        }

        var range = maxHeight - minHeight;

        foreach (var (position, height) in heightValues) {
            var color = ((height - minHeight) / range);
            var value = (byte) (color * 255);
            yield return new ColorEntry(position, Color.FromRgb(value, value, value));
        }
    }

    public Bitmap GetHeightmap(IWorldspaceGetter worldspace, Size imageSize, int leftCell, int rightCell, int topCell, int bottomCell) {
        return _creator.GetMap(worldspace, imageSize, leftCell, rightCell, topCell, bottomCell);
    }
}
