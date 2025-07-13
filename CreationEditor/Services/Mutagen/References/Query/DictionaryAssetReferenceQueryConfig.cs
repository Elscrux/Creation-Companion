using CreationEditor.Services.Archive;
using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.FileSystem.Validation;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class DictionaryAssetReferenceQueryConfig<TFileParser, TCache, TLink>(
    Func<IReadOnlyList<string>, DictionaryReferenceCacheSerialization<IDataSource, TCache, TLink, DataRelativePath>> serializationFactory,
    Func<IEnumerable<string>, IFileSystemValidation> fileSystemValidationFactory,
    IDataSourceService dataSourceService,
    IArchiveService archiveService,
    ReferenceCacheBuilder referenceCacheBuilder,
    TFileParser fileParser)
    : IReferenceQueryConfig<IDataSource, DataSourceLink, TCache, TLink>
    where TFileParser : IFileParser<TLink>
    where TCache : IDictionaryReferenceCache<TCache, TLink, DataRelativePath>
    where TLink : notnull {
    private readonly FileSystemQuery<TCache, TLink> _nifFileSystemQuery = new(fileParser, dataSourceService);
    private readonly ArchiveQuery<TCache, TLink> _nifArchiveQuery = new(fileParser, archiveService);
    private readonly DictionaryReferenceCacheSerialization<IDataSource, TCache, TLink, DataRelativePath> _serialization = serializationFactory(["References"]);
    private readonly IInternalCacheValidation<IDataSource, DataRelativePath> _cacheValidation = fileSystemValidationFactory(fileParser.FileExtensions);

    public bool CanGetLinksFromDeletedElement => false;
    public string Name => fileParser.Name;

    public Task<TCache> BuildCache(IDataSource source) {
        if (source is ArchiveDataSource archiveDataSource) {
            return referenceCacheBuilder.BuildCache(archiveDataSource, _nifArchiveQuery, _serialization, _cacheValidation);
        }

        return referenceCacheBuilder.BuildCache(source, _nifFileSystemQuery, _serialization, _cacheValidation);
    }

    public IEnumerable<TLink> GetLinks(DataSourceLink element) {
        return fileParser.ParseFile(element.FullPath, element.FileSystem);
    }
}
