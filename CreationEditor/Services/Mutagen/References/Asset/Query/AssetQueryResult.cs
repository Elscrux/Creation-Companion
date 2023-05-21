using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed record AssetQueryResult<TReference>(IAssetLinkGetter AssetLink, TReference Reference) {
    public ModKey Origin { get; init; } = ModKey.Null;
}
