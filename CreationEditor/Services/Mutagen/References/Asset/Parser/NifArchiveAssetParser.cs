using System.IO.Abstractions;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Archives;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public sealed class NifArchiveAssetParser : IArchiveAssetParser {
    private readonly IFileSystem _fileSystem;
    private readonly IAssetTypeService _assetTypeService;
    private readonly ModelAssetQuery _modelAssetQuery;
    private readonly IArchiveService _archiveService;

    public string Name => "NifArchive";

    public NifArchiveAssetParser(
        IFileSystem fileSystem,
        IAssetTypeService assetTypeService,
        ModelAssetQuery modelAssetQuery,
        IArchiveService archiveService) {
        _fileSystem = fileSystem;
        _assetTypeService = assetTypeService;
        _modelAssetQuery = modelAssetQuery;
        _archiveService = archiveService;
    }

    public IEnumerable<AssetQueryResult<string>> ParseAssets(string archivePath) {
        var archiveReader = _archiveService.GetReader(archivePath);

        foreach (var archiveFile in archiveReader.Files) {
            foreach (var result in ParseArchiveFile(archiveFile)) {
                yield return result;
            }
        }
    }

    public IEnumerable<AssetQueryResult<string>> ParseFile(string archive, string filePath) {
        var archiveReader = _archiveService.GetReader(archive);

        var directory = _fileSystem.Path.GetDirectoryName(filePath);
        if (directory is null) yield break;
        if (!archiveReader.TryGetFolder(directory, out var archiveDirectory)) yield break;

        var archiveFile = archiveDirectory.Files.FirstOrDefault(file => file.Path.Equals(filePath, AssetCompare.PathComparison));
        if (archiveFile is null) yield break;

        foreach (var result in ParseArchiveFile(archiveFile)) {
            yield return result;
        }
    }

    private IEnumerable<AssetQueryResult<string>> ParseArchiveFile(IArchiveFile archiveFile) {
        var filePath = archiveFile.Path;

        var assetType = _assetTypeService.GetAssetType(filePath);
        if (assetType != _assetTypeService.Provider.Model) yield break;

        var tempPath = _fileSystem.Path.GetTempFileName();
        try {
            //Create temp file and copy file from bsa to it
            using var bsaStream = _fileSystem.File.Create(tempPath);
            archiveFile.AsStream().CopyTo(bsaStream);
            bsaStream.Close();

            //Parse temp file as nif and delete it afterwards
            foreach (var result in _modelAssetQuery.ParseAssets(tempPath)) {
                yield return result with { Reference = filePath };
            }
        } finally {
            _fileSystem.File.Delete(tempPath);
        }
    }
}
