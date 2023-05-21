using CreationEditor.Services.Mutagen.References.Asset;
namespace CreationEditor.Services.Asset;

public sealed class AssetFile : IAsset {
    public IReferencedAsset ReferencedAsset { get; }
    public string Path { get; }
    public IEnumerable<IAsset> Children { get; } = Array.Empty<IAsset>();

    public bool IsDirectory => false;
    public bool HasChildren => false;
    public bool IsVirtual { get; }

    public AssetFile(string path, IReferencedAsset referencedAsset, bool isVirtual = false) {
        Path = path;
        ReferencedAsset = referencedAsset;
        IsVirtual = isVirtual;
    }

    public IEnumerable<IReferencedAsset> GetReferencedAssets() {
        yield return ReferencedAsset;
    }

    public void Dispose() {}
}
