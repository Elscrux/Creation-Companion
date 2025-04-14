using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Controller;

public interface IAssetReferenceController : IReferenceController<RecordModPair>, IReferenceController<FileSystemLink>, IDisposable {
    /// <summary>
    /// Observable that emits true when the controller is currently loading references.
    /// </summary>
    new IObservable<bool> IsLoading { get; }

    /// <summary>
    /// Get a decorated asset that always contains the latest information about references to this asset. 
    /// </summary>
    /// <param name="asset">Asset to get referenced asset version of</param>
    /// <param name="referencedAsset">Out variable that contains the referenced asset</param>
    /// <returns>Disposable that can be disposed to stop updates to the referenced asset</returns>
    IDisposable GetReferencedAsset(IAssetLinkGetter asset, out IReferencedAsset referencedAsset);

    /// <summary>
    /// Get the record references for this asset at the current time.
    /// </summary>
    /// <param name="assetLink">Asset link to get referenced asset version of</param>
    /// <returns>Record references for this asset</returns>
    IEnumerable<IFormLinkIdentifier> GetRecordReferences(IAssetLinkGetter assetLink);

    /// <summary>
    /// Get the asset references for this asset at the current time.
    /// </summary>
    /// <param name="assetLink">Asset link to get referenced asset version of</param>
    /// <returns>Asset references for this asset</returns>
    IEnumerable<DataRelativePath> GetAssetReferences(IAssetLinkGetter assetLink);
}
