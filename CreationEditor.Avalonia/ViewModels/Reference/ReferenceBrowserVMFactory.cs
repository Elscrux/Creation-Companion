using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.FormLink;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Avalonia.ViewModels.Reference;

public sealed class ReferenceBrowserVMFactory(
    Func<object?, IReadOnlyList<IReferenceVM>, ReferenceBrowserVM> referenceBrowserFactory,
    Func<DataRelativePath, AssetReferenceVM> assetReferenceVMFactory,
    Func<IFormLinkIdentifier, RecordReferenceVM> recordReferenceVMFactory,
    ILinkCacheProvider linkCacheProvider,
    IReferenceService referenceService,
    IAssetTypeService assetTypeService) : IReferenceBrowserVMFactory {

    public ReferenceBrowserVM? GetReferenceBrowserVM(params IReadOnlyList<IDataSourceLink> assets) {
        var recordReferences = new HashSet<IFormLinkIdentifier>(FormLinkIdentifierEqualityComparer.Instance);
        var assetReferences = new HashSet<DataRelativePath>();

        // Gather all references to all assets
        foreach (var asset in assets) {
            if (!asset.Exists()) continue;

            if (asset is DataSourceDirectoryLink directoryLink) {
                foreach (var fileLink in directoryLink.EnumerateFileLinks(true)) {
                    AddReferences(fileLink);
                }
            } else {
                AddReferences(asset);
            }

            void AddReferences(IDataSourceLink fileLink) {
                var assetLink = assetTypeService.GetAssetLink(fileLink.DataRelativePath);
                if (assetLink is null) return;

                assetReferences.AddRange(referenceService.GetAssetReferences(assetLink));
                recordReferences.AddRange(referenceService.GetRecordReferences(assetLink));
            }
        }

        var references = assetReferences
            .Select(path => new AssetReferenceVM(path, linkCacheProvider, assetTypeService, referenceService))
            .Cast<IReferenceVM>()
            .Combine(recordReferences.Select(x => new RecordReferenceVM(x, linkCacheProvider, referenceService)))
            .ToArray();

        if (references.Length == 0) return null;

        return referenceBrowserFactory(assets.Count == 1 ? assets[0] : assets, references);
    }

    public ReferenceBrowserVM? GetReferenceBrowserVM(IReferenced referencedAsset) {
        var references = referencedAsset.RecordReferences.Select(recordReferenceVMFactory)
            .Concat<IReferenceVM>(referencedAsset.AssetReferences.Select(assetReferenceVMFactory))
            .ToArray();

        if (references.Length == 0) return null;

        return referenceBrowserFactory(referencedAsset, references);
    }
}
