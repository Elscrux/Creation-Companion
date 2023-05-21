using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Asset;

public interface IAssetTypeProvider {
    public IReadOnlyList<IAssetType> AllAssetTypes { get; }
    public IDictionary<IAssetType, Func<string, IAssetLink>> AssetTypeConstructor { get; }
    public IDictionary<IAssetType, string> AssetTypeIdentifiers { get; }

    IAssetType Texture { get; }
    IAssetType Model { get; }
    IAssetType ScriptSource { get; }
    IAssetType Script { get; }
    IAssetType Sound { get; }
    IAssetType Music { get; }
}
