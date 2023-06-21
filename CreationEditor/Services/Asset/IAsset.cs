using CreationEditor.Services.Mutagen.References.Asset;
namespace CreationEditor.Services.Asset;

public interface IAsset : IDisposable {
    string Path { get; }
    IEnumerable<IAsset> Children { get; }
    bool IsDirectory { get; }
    bool HasChildren { get; }
    bool IsVirtual { get; }

    /// <returns>IReferencedAsset versions of this asset and all its children</returns>
    IEnumerable<IReferencedAsset> GetReferencedAssets();
}
