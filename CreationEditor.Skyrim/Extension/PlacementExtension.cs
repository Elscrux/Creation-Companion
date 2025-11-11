using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim;

public static class PlacementExtension
{
    extension(IPlacementGetter placement) {
        public P2Int GetCellCoordinates()
        {
            const int cellLength = 4096;
            var position = placement.Position;

            return new P2Int(ToInt(position.X), ToInt(position.Y));

            int ToInt(float pos) => (int)Math.Floor(pos / cellLength);
        }
    }
}
