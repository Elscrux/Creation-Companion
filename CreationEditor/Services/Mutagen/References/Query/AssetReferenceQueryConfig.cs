using CreationEditor.Services.Archive;
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
    AssetReferenceCacheSerialization<IDataSource, DataRelativePath> serialization,
    IArchiveService archiveService,
    ReferenceCacheBuilder referenceCacheBuilder,
    TFileParser fileParser)
    : IReferenceQueryConfig<IDataSource, DataSourceLink, AssetReferenceCache<DataRelativePath>, IAssetLinkGetter>
    where TFileParser : IFileParser<IAssetLinkGetter> {
    private readonly FileSystemQuery<AssetReferenceCache<DataRelativePath>, IAssetLinkGetter> _nifFileSystemQuery = new(fileParser, dataSourceService);
    private readonly ArchiveQuery<AssetReferenceCache<DataRelativePath>, IAssetLinkGetter> _nifArchiveQuery = new(fileParser, archiveService);
    private readonly IInternalCacheValidation<IDataSource, DataRelativePath> _cacheValidation = fileSystemValidationFactory(fileParser.FileExtensions);

    public bool CanGetLinksFromDeletedElement => false;
    public string Name => fileParser.Name;

    public Task<AssetReferenceCache<DataRelativePath>> BuildCache(IDataSource source) {
        if (source is ArchiveDataSource archiveDataSource) {
            return referenceCacheBuilder.BuildCache(archiveDataSource, _nifArchiveQuery, serialization, _cacheValidation);
        }

        return referenceCacheBuilder.BuildCache(source, _nifFileSystemQuery, serialization, _cacheValidation);
    }

    public IEnumerable<IAssetLinkGetter> GetLinks(DataSourceLink element) {
        return fileParser.ParseFile(element.FullPath, element.FileSystem);
    }
}
