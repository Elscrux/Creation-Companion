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

    public DataSourceFileLink? ReferenceToSource(DataRelativePath reference) {
        return dataSourceService.TryGetFileLink(reference.Path, out var fileLink) ? fileLink : null;
    }

    public void FillCache(FileSystemDataSource source, TCache cache) {
        var rootLink = source.GetRootLink();

        // Parse all files with the specified extensions
        foreach (var fileExtension in fileParser.AssetType.FileExtensions) {
            var searchPattern = "*" + fileExtension;
            foreach (var file in rootLink.EnumerateFileLinks(searchPattern, true)) {
                var assetType = assetTypeService.GetAssetType(file.DataRelativePath);
                if (fileParser.AssetType != assetType) continue;

                using var stream = file.ReadFileStream();
                foreach (var result in fileParser.ParseFile(stream, file)) {
                    cache.Add(result, file.DataRelativePath);
                }
            }
        }
    }

    public void FillCache(DataRelativePath reference, TCache cache) {
        if (!dataSourceService.TryGetFileLink(reference.Path, out var fileLink)) return;

        var assetType = assetTypeService.GetAssetType(fileLink.DataRelativePath);
        if (fileParser.AssetType != assetType) return;

        using var stream = fileLink.ReadFileStream();
        foreach (var result in fileParser.ParseFile(stream, fileLink)) {
            cache.Add(result, fileLink.DataRelativePath);
        }
    }
}
