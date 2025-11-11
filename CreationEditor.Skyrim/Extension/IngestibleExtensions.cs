using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class IngestibleExtensions {
    extension(IIngestibleGetter ingestible) {
        public bool IsPoison() {
            return ingestible.Flags.HasFlag(Ingestible.Flag.Poison);
        }
        public bool IsFood() {
            return ingestible.Flags.HasFlag(Ingestible.Flag.FoodItem);
        }
        public bool IsPotion() {
            return (ingestible.Flags & (Ingestible.Flag.Poison | Ingestible.Flag.FoodItem | Ingestible.Flag.Medicine)) == 0;
        }
    }

}
