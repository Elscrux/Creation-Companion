namespace CreationEditor.Services.Asset;

public interface IAssetProvider {
    IAsset GetAssetContainer(string directory);
}