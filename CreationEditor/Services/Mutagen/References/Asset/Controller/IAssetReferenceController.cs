using CreationEditor.Services.Asset;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Asset.Controller;

public interface IAssetReferenceController : IReferenceController<IMajorRecordGetter>, IReferenceController<AssetFile>, IDisposable {
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
    IDisposable GetReferencedAsset(IAssetLink asset, out IReferencedAsset referencedAsset);
}
