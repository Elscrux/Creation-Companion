using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class ModExtension {
    public static uint GetRecordCount(this IModGetter mod) {
        return mod switch {
            ISkyrimModGetter skyrimMod => skyrimMod.ModHeader.Stats.NumRecords,
            _ => throw new ArgumentOutOfRangeException(nameof(mod))
        };
    }
}
