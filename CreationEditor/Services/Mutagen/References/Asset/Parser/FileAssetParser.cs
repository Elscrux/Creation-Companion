using System.IO.Abstractions;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Query;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public sealed class FileAssetParser : IFileAssetParser {
    private readonly IFileSystem _fileSystem;
    private readonly IAssetTypeService _assetTypeService;
    private readonly IArchiveService _archiveService;

    public string Name => "File";

    public FileAssetParser(
        IFileSystem fileSystem,
        IAssetTypeService assetTypeService,
        IArchiveService archiveService) {
        _fileSystem = fileSystem;
        _assetTypeService = assetTypeService;
        _archiveService = archiveService;
    }

    public IEnumerable<AssetQueryResult<string>> ParseFile(string filePath) {
        var type = _assetTypeService.GetAssetType(filePath);

        if (type is not null) {
            yield return new AssetQueryResult<string>(_assetTypeService.GetAssetLink(filePath, type), filePath);
        } else if (_fileSystem.Path.GetExtension(filePath).Equals(_archiveService.GetExtension(), AssetCompare.PathComparison)) {
            //BSAPath
            // using var bsaAssetManager = new DirectoryArchiveAssetQuery(file);
            // SubManagers.Add(bsaAssetManager);
            // if (BSAParsing) {
            //     bsaAssetManager.ParseAssets();
            // }
        }
    }
}
