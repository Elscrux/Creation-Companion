using Mutagen.Bethesda.Plugins.Assets;
namespace BuildStripper.Models;

public sealed record AssetLinkIdentifier(IAssetLinkGetter AssetLink) : ILinkIdentifier {
    public override string ToString() {
        return AssetLink.DataRelativePath.ToString();
    }

    public bool IsNull => AssetLink.IsNull;
}
