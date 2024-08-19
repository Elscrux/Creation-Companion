using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Asset;

public sealed class AssetLinkEqualityComparer : IEqualityComparer<IAssetLinkGetter> {
    public static readonly AssetLinkEqualityComparer Instance = new();

    public bool Equals(IAssetLinkGetter? x, IAssetLinkGetter? y) {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        return x.DataRelativePath.Equals(y.DataRelativePath);
    }

    public int GetHashCode(IAssetLinkGetter obj) {
        return obj.DataRelativePath.GetHashCode();
    }
}
