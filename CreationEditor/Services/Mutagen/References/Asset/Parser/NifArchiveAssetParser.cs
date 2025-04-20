using System.IO.Abstractions;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

// TODO: replace with with generic implementation that doesn't require special handling of archives 
public sealed class NifArchiveAssetParser(
    IAssetTypeService assetTypeService,
    ModelAssetQuery modelAssetQuery,
    IArchiveService archiveService)
    : IArchiveAssetParser {

    public string Name => "NifArchive";

    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(FileSystemLink archiveLink) {
        var archiveReader = archiveService.GetReader(archiveLink);

        foreach (var archiveFile in archiveReader.Files) {
            foreach (var result in ParseArchiveFile(archiveFile, archiveLink.FileSystem)) {
                yield return result;
            }
        }
    }

    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseFile(FileSystemLink archive, string filePath) {
        var archiveReader = archiveService.GetReader(archive);

        var directory = archive.FileSystem.Path.GetDirectoryName(filePath);
        if (directory is null) yield break;
        if (!archiveReader.TryGetFolder(directory, out var archiveDirectory)) yield break;

        var archiveFile = archiveDirectory.Files.FirstOrDefault(file => file.Path.Equals(filePath, DataRelativePath.PathComparison));
        if (archiveFile is null) yield break;

        foreach (var result in ParseArchiveFile(archiveFile, archive.FileSystem)) {
            yield return result;
        }
    }

    private IEnumerable<AssetQueryResult<DataRelativePath>> ParseArchiveFile(IArchiveFile archiveFile, IFileSystem fileSystem) {
        var assetType = assetTypeService.GetAssetType(archiveFile.Path);
        if (assetType != assetTypeService.Provider.Model) yield break;

        var tempPath = fileSystem.Path.GetTempFileName();
        try {
            //Create a temp file and copy the file from the archive to it
            using var archiveStream = fileSystem.File.Create(tempPath);
            archiveFile.AsStream().CopyTo(archiveStream);
            archiveStream.Close();

            //Parse the temp file as nif and delete it afterward
            foreach (var result in modelAssetQuery.ParseAssets(tempPath, fileSystem, archiveFile.Path)) {
                yield return result;
            }
        } finally {
            fileSystem.File.Delete(tempPath);
        }
    }
}
