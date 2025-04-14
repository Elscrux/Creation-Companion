// using CreationEditor.Services.DataSource;
// using CreationEditor.Services.Mutagen.References.Asset;
// namespace CreationEditor.Services.Asset;
//
// public sealed class AssetFile(FileSystemLink link, IReferencedAsset referencedAsset, bool isVirtual = false) : IAsset {
//     public FileSystemLink Link { get; } = link;
//     public IReferencedAsset ReferencedAsset { get; } = referencedAsset;
//     public IEnumerable<IAsset> Children { get; } = [];
//
//     public bool IsDirectory => false;
//     public bool HasChildren => false;
//     public bool IsVirtual { get; } = isVirtual;
//
//     public IEnumerable<IReferencedAsset> GetReferencedAssets() {
//         yield return ReferencedAsset;
//     }
//
//     public void Dispose() {}
// }
