using Mutagen.Bethesda.Assets;
using Noggog;
namespace CreationEditor.Resources.Comparer;

public sealed class DataRelativePathComparer() : FuncEqualityComparer<DataRelativePath>(
    (a, b) => DataRelativePath.PathComparer.Equals(a, b),
    a => DataRelativePath.PathComparer.GetHashCode(a)) {
    public static DataRelativePathComparer Instance { get; } = new();
}
