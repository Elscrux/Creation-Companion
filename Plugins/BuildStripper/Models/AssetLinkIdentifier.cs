using Mutagen.Bethesda.Plugins.Assets;
namespace BuildStripper.Models;

public record AssetLinkIdentifier(IAssetLinkGetter AssetLink) : ILinkIdentifier {
    public override string ToString() {
        return AssetLink.DataRelativePath.ToString();
    }
}
