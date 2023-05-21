using Autofac;
using CreationEditor.Services.Asset;
using Mutagen.Bethesda.Archives;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class NifArchiveAssetQuery : ArchiveAssetQuery {
    private readonly ModelAssetQuery _modelAssetQuery;

    protected override string QueryName => "NifArchive";
    protected override bool CacheAssets => true;

    public NifArchiveAssetQuery(
        ILifetimeScope lifetimeScope) : base(lifetimeScope) {
        var newScope = lifetimeScope.BeginLifetimeScope().DisposeWith(this);
        _modelAssetQuery = newScope.Resolve<ModelAssetQuery>();
    }

    public override IEnumerable<AssetQueryResult<string>> ParseAssets(string archive) {
        var archiveReader = ArchiveService.GetReader(archive);

        foreach (var archiveFile in archiveReader.Files) {
            foreach (var result in ParseArchiveFile(archiveFile)) {
                yield return result;
            }
        }
    }

    public IEnumerable<AssetQueryResult<string>> ParseFile(string archive, string filePath) {
        var archiveReader = ArchiveService.GetReader(archive);

        var directory = FileSystem.Path.GetDirectoryName(filePath);
        if (directory == null) yield break;
        if (!archiveReader.TryGetFolder(directory, out var archiveDirectory)) yield break;

        var archiveFile = archiveDirectory.Files.FirstOrDefault(file => file.Path.Equals(filePath, AssetCompare.PathComparison));
        if (archiveFile == null) yield break;

        foreach (var result in ParseArchiveFile(archiveFile)) {
            yield return result;
        }
    }

    private IEnumerable<AssetQueryResult<string>> ParseArchiveFile(IArchiveFile archiveFile) {
        var filePath = archiveFile.Path;

        var assetType = AssetTypeService.GetAssetType(filePath);
        if (assetType != AssetTypeService.Provider.Model) yield break;

        var tempPath = FileSystem.Path.GetTempFileName();
        try {
            //Create temp file and copy file from bsa to it
            using var bsaStream = FileSystem.File.Create(tempPath);
            archiveFile.AsStream().CopyTo(bsaStream);
            bsaStream.Close();

            //Parse temp file as nif and delete it afterwards
            foreach (var result in _modelAssetQuery.ParseAssets(tempPath)) {
                yield return result with { Reference = filePath };
            }
        } finally {
            FileSystem.File.Delete(tempPath);
        }
    }
}
