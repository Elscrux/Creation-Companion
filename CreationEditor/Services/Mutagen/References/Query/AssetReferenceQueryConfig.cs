using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.FileSystem.Validation;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Parser;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class AssetReferenceCacheQueryConfig<TFileParser>(
    Func<IEnumerable<string>, IFileSystemValidation> fileSystemValidationFactory,
    IDataSourceService dataSourceService,
    AssetReferenceCacheSerialization<FileSystemDataSource, DataRelativePath> fileSystemSerialization,
    AssetReferenceCacheSerialization<ArchiveDataSource, DataRelativePath> archiveSerialization,
    IArchiveService archiveService,
    IAssetTypeService assetTypeService,
    ReferenceCacheBuilder referenceCacheBuilder,
    TFileParser fileParser)
    : IReferenceQueryConfig<IDataSource, DataSourceFileLink, AssetReferenceCache<DataRelativePath>, IAssetLinkGetter>
    where TFileParser : IFileParser<IAssetLinkGetter> {
    private readonly FileSystemQuery<AssetReferenceCache<DataRelativePath>, IAssetLinkGetter> _nifFileSystemQuery =
        new(fileParser, assetTypeService, dataSourceService);
    private readonly ArchiveQuery<AssetReferenceCache<DataRelativePath>, IAssetLinkGetter> _nifArchiveQuery =
        new(fileParser, assetTypeService, archiveService);
    private readonly IInternalCacheValidation<FileSystemDataSource, DataRelativePath> _cacheValidation =
        fileSystemValidationFactory(fileParser.AssetType.FileExtensions);

    public bool CanGetLinksFromDeletedElement => false;
    public string Name => fileParser.Name;

    public Task<AssetReferenceCache<DataRelativePath>> BuildCache(IDataSource source) {
        return source switch {
            FileSystemDataSource fileSystemDataSource =>
                referenceCacheBuilder.BuildCache(fileSystemDataSource, _nifFileSystemQuery, fileSystemSerialization, _cacheValidation),
            ArchiveDataSource archiveDataSource =>
                referenceCacheBuilder.BuildCache(archiveDataSource, _nifArchiveQuery, archiveSerialization),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    public IEnumerable<IAssetLinkGetter> GetLinks(DataSourceFileLink element) {
        var extension = element.Extension;
        if (!fileParser.AssetType.FileExtensions.Contains(extension)) return [];

        return fileParser.ParseFile(element.FullPath, element.FileSystem);
    }
}
