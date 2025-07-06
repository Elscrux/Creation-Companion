using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class IngestibleExtensions {
    public static bool IsPoison(this IIngestibleGetter ingestible) {
        return ingestible.Flags.HasFlag(Ingestible.Flag.Poison);
    }

    public static bool IsFood(this IIngestibleGetter ingestible) {
        return ingestible.Flags.HasFlag(Ingestible.Flag.FoodItem);
    }

    public static bool IsPotion(this IIngestibleGetter ingestible) {
        return (ingestible.Flags & (Ingestible.Flag.Poison | Ingestible.Flag.FoodItem | Ingestible.Flag.Medicine)) == 0;
    }
}
