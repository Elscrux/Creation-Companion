using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Asset;
namespace CreationEditor.Avalonia.Comparer;

public static class AssetComparers {
    public static readonly FuncComparer<IAsset> PathComparer = new((a1, a2) => {
        if (a1.IsDirectory == a2.IsDirectory) {
            return string.Compare(a1.Path, a2.Path, AssetCompare.PathComparison);
        }

        if (a1.IsDirectory) return -1;

        return 1;
    });

    public static readonly FuncComparer<IAsset> ReferenceCountComparer = new((a1, a2) => {
        return a1.GetReferencedAssets()
            .Select(x => x.RecordReferences.Count + x.AssetReferences.Count)
            .Sum()
            .CompareTo(a2.GetReferencedAssets()
                .Select(x => x.RecordReferences.Count + x.AssetReferences.Count)
                .Sum());
    });
}
