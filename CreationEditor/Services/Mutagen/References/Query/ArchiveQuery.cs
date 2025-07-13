using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class ArchiveQuery<TCache, TLink>(
    IFileParser<TLink> fileParser,
    IAssetTypeService assetTypeService,
    IArchiveService archiveService)
    : IReferenceQuery<ArchiveDataSource, TCache, TLink, DataRelativePath>
    where TCache : IReferenceCache<TCache, TLink, DataRelativePath>
    where TLink : notnull {
    public string Name => fileParser.Name + " (Archive)";
    public string GetSourceName(ArchiveDataSource source) => source.Name;
    public ArchiveDataSource? ReferenceToSource(DataRelativePath reference) => null;
    public void FillCache(ArchiveDataSource source, TCache cache) {
        var archiveReader = archiveService.GetReader(source.GetRootLink());

        foreach (var archiveFile in archiveReader.Files) {
            var assetType = assetTypeService.GetAssetType(archiveFile.Path);
            if (fileParser.AssetType != assetType) continue;

            var extension = source.FileSystem.Path.GetExtension(archiveFile.Path);
            if (!fileParser.AssetType.FileExtensions.Contains(extension, DataRelativePath.PathComparer)) continue;

            var fileSystem = source.FileSystem;
            var tempPath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
            try {
                //Create a temp file and copy the file from the archive to it
                using var archiveStream = fileSystem.File.Create(tempPath);
                archiveFile.AsStream().CopyTo(archiveStream);
                archiveStream.Close();

                //Parse the temp file as nif and delete it afterward
                foreach (var result in fileParser.ParseFile(tempPath, fileSystem)) {
                    cache.Add(result, archiveFile.Path);
                }
            } finally {
                fileSystem.File.Delete(tempPath);
            }
        }
    }
}
