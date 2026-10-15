using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class ArchiveQuery<TCache, TLink>(
    IFileParser<TLink> fileParser,
    IAssetTypeService assetTypeService)
    : IReferenceQuery<ArchiveDataSource, TCache, TLink, DataRelativePath>
    where TCache : IReferenceCache<TCache, TLink, DataRelativePath>
    where TLink : notnull {
    public string Name => fileParser.Name + " (Archive)";
    public string GetSourceName(ArchiveDataSource source) => source.Name;
    public void FillCache(ArchiveDataSource source, TCache cache) {
        foreach (var archiveFile in source.ArchiveReader.Files) {
            var assetType = assetTypeService.GetAssetType(archiveFile.Path);
            if (fileParser.AssetType != assetType) continue;

            var extension = source.FileSystem.Path.GetExtension(archiveFile.Path);
            if (!fileParser.AssetType.FileExtensions.Contains(extension, DataRelativePath.PathComparer)) continue;

            var fileLink = new DataSourceFileLink(source, new DataRelativePath(archiveFile.Path));
            using var stream = fileLink.ReadFileStream();
            foreach (var result in fileParser.ParseFile(stream, fileLink)) {
                cache.Add(result, archiveFile.Path);
            }
        }
    }

    public void FillCache(DataRelativePath reference, TCache cache) {}
}
