using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Asset;

public interface IAssetTypeProvider {
    /// <summary>
    /// List of all asset types
    /// </summary>
    IReadOnlyList<IAssetType> AllAssetTypes { get; }

    /// <summary>
    /// Map for constructors of asset links for each asset type
    /// </summary>
    IDictionary<IAssetType, Func<DataRelativePath, IAssetLink>> AssetTypeConstructor { get; }

    /// <summary>
    /// Map for identifiers of asset types. Each identifier is a three character string.
    /// </summary>
    IDictionary<IAssetType, string> AssetTypeIdentifiers { get; }

    IAssetType Texture { get; }
    IAssetType Model { get; }
    IAssetType ScriptSource { get; }
    IAssetType Script { get; }
    IAssetType Sound { get; }
    IAssetType Music { get; }
    IAssetType DeformedModel { get; }
    IAssetType Seq { get; }
    IAssetType BodyTexture { get; }
    IAssetType Behavior { get; }
    IAssetType Translation { get; }
    IAssetType Interface { get; }
}
