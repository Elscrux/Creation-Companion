using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed record AssetQueryResult<TReference>(IAssetLinkGetter AssetLink, TReference Reference);
