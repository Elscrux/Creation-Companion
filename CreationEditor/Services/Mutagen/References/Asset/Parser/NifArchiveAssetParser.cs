using System.IO.Abstractions;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public sealed class NifArchiveAssetParser(
    IFileSystem fileSystem,
    IAssetTypeService assetTypeService,
    ModelAssetQuery modelAssetQuery,
    IArchiveService archiveService)
    : IArchiveAssetParser {

    public string Name => "NifArchive";

    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(string archivePath) {
        var archiveReader = archiveService.GetReader(archivePath);

        foreach (var archiveFile in archiveReader.Files) {
            foreach (var result in ParseArchiveFile(archiveFile)) {
                yield return result;
            }
        }
    }

    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseFile(string archive, string filePath) {
        var archiveReader = archiveService.GetReader(archive);

        var directory = fileSystem.Path.GetDirectoryName(filePath);
        if (directory is null) yield break;
        if (!archiveReader.TryGetFolder(directory, out var archiveDirectory)) yield break;

        var archiveFile = archiveDirectory.Files.FirstOrDefault(file => file.Path.Equals(filePath, AssetCompare.PathComparison));
        if (archiveFile is null) yield break;

        foreach (var result in ParseArchiveFile(archiveFile)) {
            yield return result;
        }
    }

    private IEnumerable<AssetQueryResult<DataRelativePath>> ParseArchiveFile(IArchiveFile archiveFile) {
        var assetType = assetTypeService.GetAssetType(archiveFile.Path);
        if (assetType != assetTypeService.Provider.Model) yield break;

        var tempPath = fileSystem.Path.GetTempFileName();
        try {
            //Create temp file and copy file from bsa to it
            using var bsaStream = fileSystem.File.Create(tempPath);
            archiveFile.AsStream().CopyTo(bsaStream);
            bsaStream.Close();

            //Parse temp file as nif and delete it afterward
            foreach (var result in modelAssetQuery.ParseAssets(tempPath, archiveFile.Path)) {
                yield return result;
            }
        } finally {
            fileSystem.File.Delete(tempPath);
        }
    }
}
