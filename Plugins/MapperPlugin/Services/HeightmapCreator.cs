using Avalonia.Media.Imaging;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Size = Avalonia.Size;
namespace MapperPlugin.Services;

public sealed class HeightmapCreator {
    public readonly record struct HeightEntry(P2Int Position, float Height);

    private readonly WorldMapCreator _creator = new(GetHeightColors);

    private static IEnumerable<HeightEntry> GetHeightValues(IWorldspaceGetter worldspace) {
        foreach (var cell in worldspace.EnumerateMajorRecords<ICellGetter>()) {
            if (cell.Landscape is null) continue;
            if (cell.Grid is null) continue;
            if (cell.Landscape.VertexHeightMap is null) continue;

            var cellPosition = cell.Grid.Point * WorldMapCreator.LandscapePoints;
            foreach (var height in cell.Landscape.VertexHeightMap.HeightMap) {
                var heightValue = cell.Landscape.VertexHeightMap.Offset + height.Value * 8;
                yield return new HeightEntry(cellPosition + height.Key, heightValue);
            }
        }
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
            var color = new P3Float(1, 1, 1) * ((height - minHeight) / range);
            yield return new ColorEntry(position, new P3UInt8((byte) (color.X * 255), (byte) (color.Y * 255), (byte) (color.Z * 255)));
        }
    }

    public Bitmap GetHeightmap(IWorldspaceGetter worldspace, Size imageSize, int leftCell, int rightCell, int topCell, int bottomCell) {
        return _creator.GetMap(worldspace, imageSize, leftCell, rightCell, topCell, bottomCell);
    }
}
