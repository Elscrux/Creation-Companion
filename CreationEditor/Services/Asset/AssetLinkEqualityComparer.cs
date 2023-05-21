using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Asset;

public sealed class AssetLinkEqualityComparer : IEqualityComparer<IAssetLinkGetter> {
    public static readonly AssetLinkEqualityComparer Instance = new();

    public bool Equals(IAssetLinkGetter? x, IAssetLinkGetter? y) {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;

        return string.Equals(x.DataRelativePath, y.DataRelativePath, AssetCompare.PathComparison);
    }

    public int GetHashCode(IAssetLinkGetter obj) {
        return HashCode.Combine(obj.DataRelativePath.ToLowerInvariant());
    }
}
