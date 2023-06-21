using Autofac;
using CreationEditor.Services.Asset;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class DirectoryAssetQuery : FileStructureAssetQuery {
    protected override string QueryName => "Directory";
    protected override bool CacheAssets => false;

    public DirectoryAssetQuery(ILifetimeScope lifetimeScope) : base(lifetimeScope) {}

    public override IEnumerable<AssetQueryResult<string>> ParseFile(string directory) {
        var type = AssetTypeService.GetAssetType(directory);

        if (type is not null) {
            yield return new AssetQueryResult<string>(AssetTypeService.GetAssetLink(directory, type), directory);
        } else if (FileSystem.Path.GetExtension(directory).Equals(ArchiveService.GetExtension(), AssetCompare.PathComparison)) {
            //BSAPath
            // using var bsaAssetManager = new DirectoryArchiveAssetQuery(file);
            // SubManagers.Add(bsaAssetManager);
            // if (BSAParsing) {
            //     bsaAssetManager.ParseAssets();
            // }
        }
    }
}
