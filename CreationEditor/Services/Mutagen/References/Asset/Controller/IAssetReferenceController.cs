using CreationEditor.Services.Asset;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Asset.Controller;

public interface IAssetReferenceController : IReferenceController<IMajorRecordGetter>, IReferenceController<AssetFile>, IDisposable {
    IObservable<bool> IsLoading { get; }

    IDisposable GetReferencedAsset(IAssetLink asset, out IReferencedAsset referencedAsset);
}
