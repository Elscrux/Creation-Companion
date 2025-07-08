using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public record struct AssetQueryResult<TReference>(IAssetLinkGetter AssetLink, TReference Reference);
