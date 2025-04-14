using System.IO.Abstractions;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

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
            //Create temp file and copy file from bsa to it
            using var bsaStream = fileSystem.File.Create(tempPath);
            archiveFile.AsStream().CopyTo(bsaStream);
            bsaStream.Close();

            //Parse temp file as nif and delete it afterward
            foreach (var result in modelAssetQuery.ParseAssets(tempPath, fileSystem, archiveFile.Path)) {
                yield return result;
            }
        } finally {
            fileSystem.File.Delete(tempPath);
        }
    }
}
