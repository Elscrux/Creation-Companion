using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Asset;

public interface IAssetTypeService {
    IReadOnlyList<string> FileExtensions { get; }

    IAssetTypeProvider Provider { get; }

    IAssetType? GetAssetType(string file);
    IAssetLink? GetAssetLink(string file);
    IAssetLink GetAssetLink(string file, IAssetType assetType);

    string GetAssetTypeIdentifier(IAssetType assetType);
    IAssetType GetAssetTypeFromIdentifier(string identifier);
}
