using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.FileSystem.Validation;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class DictionaryAssetReferenceQueryConfig<TFileParser, TCache, TLink>(
    Func<IReadOnlyList<string>, DictionaryReferenceCacheSerialization<FileSystemDataSource, TCache, TLink, DataRelativePath>> fileSystemSerializationFactory,
    Func<IReadOnlyList<string>, DictionaryReferenceCacheSerialization<ArchiveDataSource, TCache, TLink, DataRelativePath>> archiveSerializationFactory,
    Func<IEnumerable<string>, IFileSystemValidation> fileSystemValidationFactory,
    IDataSourceService dataSourceService,
    IArchiveService archiveService,
    IAssetTypeService assetTypeService,
    ReferenceCacheBuilder referenceCacheBuilder,
    TFileParser fileParser)
    : IReferenceQueryConfig<IDataSource, DataSourceFileLink, TCache, TLink>
    where TFileParser : IFileParser<TLink>
    where TCache : IDictionaryReferenceCache<TCache, TLink, DataRelativePath>
    where TLink : notnull {
    private readonly FileSystemQuery<TCache, TLink> _nifFileSystemQuery = new(fileParser, assetTypeService, dataSourceService);
    private readonly ArchiveQuery<TCache, TLink> _nifArchiveQuery = new(fileParser, assetTypeService, archiveService);
    private readonly DictionaryReferenceCacheSerialization<FileSystemDataSource, TCache, TLink, DataRelativePath> _fileSystemSerializationFactory =
        fileSystemSerializationFactory(["References"]);
    private readonly DictionaryReferenceCacheSerialization<ArchiveDataSource, TCache, TLink, DataRelativePath> _archiveSerializationFactory =
        archiveSerializationFactory(["References"]);
    private readonly IInternalCacheValidation<FileSystemDataSource, DataRelativePath> _cacheValidation =
        fileSystemValidationFactory(fileParser.AssetType.FileExtensions);

    public bool CanGetLinksFromDeletedElement => false;
    public string Name => fileParser.Name;

    public Task<TCache> BuildCache(IDataSource source) {
        return source switch {
            FileSystemDataSource fileSystemDataSource =>
                referenceCacheBuilder.BuildCache(fileSystemDataSource, _nifFileSystemQuery, _fileSystemSerializationFactory, _cacheValidation),
            ArchiveDataSource archiveDataSource =>
                referenceCacheBuilder.BuildCache(archiveDataSource, _nifArchiveQuery, _archiveSerializationFactory)
        };
    }

    public IEnumerable<TLink> GetLinks(DataSourceFileLink element) {
        return fileParser.ParseFile(element.FullPath, element.FileSystem);
    }
}
