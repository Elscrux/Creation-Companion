using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References;

public interface IReferenceService {
    IObservable<bool> IsLoading { get; }
    IObservable<bool> IsLoadingAssetReferences { get; }
    IObservable<bool> IsLoadingRecordReferences { get; }
    IDisposable GetReferencedAsset(IAssetLinkGetter asset, out IReferencedAsset referencedAsset);
    IDisposable GetReferencedRecord<TMajorRecordGetter>(TMajorRecordGetter record, out IReferencedRecord<TMajorRecordGetter> outReferencedRecord)
        where TMajorRecordGetter : IMajorRecordGetter;
    IEnumerable<IFormLinkIdentifier> GetRecordReferences(IFormLinkIdentifier formLink);
    IEnumerable<IFormLinkIdentifier> GetRecordReferences(IAssetLinkGetter assetLink);
    IEnumerable<DataRelativePath> GetAssetReferences(IFormLinkIdentifier formLink);
    IEnumerable<DataRelativePath> GetAssetReferences(IAssetLinkGetter assetLink);
    IEnumerable<string> GetMissingRecordLinks(FileSystemLink fileLink);
    IEnumerable<IAssetLinkGetter> GetAssetLinks(FileSystemLink fileLink);
    int GetReferenceCount(IAssetLink assetLink);
    int GetReferenceCount(IFormLinkIdentifier formLink);
}
