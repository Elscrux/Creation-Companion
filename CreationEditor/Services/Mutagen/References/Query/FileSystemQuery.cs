using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class FileSystemQuery<TCache, TLink>(
    IFileParser<TLink> fileParser,
    IDataSourceService dataSourceService)
    : IReferenceQuery<IDataSource, TCache, TLink, DataRelativePath>
    where TCache : IReferenceCache<TCache, TLink, DataRelativePath>
    where TLink : notnull {
    public string Name => fileParser.Name;

    public string GetSourceName(IDataSource source) => source.Path;

    public IDataSource? ReferenceToSource(DataRelativePath reference) => dataSourceService.TryGetDataSource(reference.Path, out var dataSource) ? dataSource : null;

    public void FillCache(IDataSource source, TCache cache) {
        var rootLink = source.GetRootLink();

        // Parse all files with the specified extensions
        foreach (var fileExtension in fileParser.FileExtensions) {
            var searchPattern = "*" + fileExtension;
            foreach (var file in rootLink.EnumerateFileLinks(searchPattern, true)) {
                foreach (var result in fileParser.ParseFile(file.FullPath, file.FileSystem)) {
                    cache.Add(result, file.DataRelativePath);
                }
            }
        }
    }
}
