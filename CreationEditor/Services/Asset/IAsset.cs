using CreationEditor.Services.Mutagen.References.Asset;
namespace CreationEditor.Services.Asset;

public interface IAsset : IDisposable {
    string Path { get; }
    IEnumerable<IAsset> Children { get; }
    bool IsDirectory { get; }
    bool HasChildren { get; }
    bool IsVirtual { get; }

    IEnumerable<IReferencedAsset> GetReferencedAssets();
}
