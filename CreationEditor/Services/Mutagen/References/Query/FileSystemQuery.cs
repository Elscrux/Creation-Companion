using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class FileSystemQuery<TCache, TLink>(
    IFileParser<TLink> fileParser,
    IAssetTypeService assetTypeService,
    IDataSourceService dataSourceService)
    : IReferenceQuery<FileSystemDataSource, TCache, TLink, DataRelativePath>
    where TCache : IReferenceCache<TCache, TLink, DataRelativePath>
    where TLink : notnull {
    public string Name => fileParser.Name;

    public string GetSourceName(FileSystemDataSource source) => source.Path;

    public FileSystemDataSource? ReferenceToSource(DataRelativePath reference) {
        return dataSourceService.TryGetDataSource(reference.Path, out var dataSource) ? dataSource as FileSystemDataSource : null;
    }

    public void FillCache(FileSystemDataSource source, TCache cache) {
        var rootLink = source.GetRootLink();

        // Parse all files with the specified extensions
        foreach (var fileExtension in fileParser.AssetType.FileExtensions) {
            var searchPattern = "*" + fileExtension;
            foreach (var file in rootLink.EnumerateFileLinks(searchPattern, true)) {
                var assetType = assetTypeService.GetAssetType(file.FullPath);
                if (fileParser.AssetType != assetType) continue;

                foreach (var result in fileParser.ParseFile(file.FullPath, file.FileSystem)) {
                    cache.Add(result, file.DataRelativePath);
                }
            }
        }
    }
}
