using CreationEditor.Services.Mutagen.References.Asset;
namespace CreationEditor.Services.Asset;

public sealed class AssetFile(string path, IReferencedAsset referencedAsset, bool isVirtual = false) : IAsset {
    public IReferencedAsset ReferencedAsset { get; } = referencedAsset;
    public string Path { get; } = path;
    public IEnumerable<IAsset> Children { get; } = [];

    public bool IsDirectory => false;
    public bool HasChildren => false;
    public bool IsVirtual { get; } = isVirtual;

    public IEnumerable<IReferencedAsset> GetReferencedAssets() {
        yield return ReferencedAsset;
    }

    public void Dispose() {}
}
