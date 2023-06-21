using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Asset;
namespace CreationEditor.Avalonia.Comparer;

public static class AssetComparers {
    public static readonly FuncComparer<IAsset> PathComparer = new((a1, a2) => {
        if (a1 is null && a2 is null) return 0;
        if (a1 is null) return -1;
        if (a2 is null) return 1;

        if (a1.IsDirectory == a2.IsDirectory) {
            return string.Compare(a1.Path, a2.Path, AssetCompare.PathComparison);
        }

        if (a1.IsDirectory) return -1;

        return 1;
    });
}
