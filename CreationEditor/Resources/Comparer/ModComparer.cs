using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Resources.Comparer;

public sealed class ModComparer() : FuncEqualityComparer<IModGetter>(
    (a, b) => a?.ModKey.Equals(b?.ModKey) is true,
    mod => mod.ModKey.GetHashCode()) {
    public static ModComparer Instance { get; } = new();
}
