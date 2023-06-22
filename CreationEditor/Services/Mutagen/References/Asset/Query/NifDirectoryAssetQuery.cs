using Autofac;
using Mutagen.Bethesda.Environments.DI;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class NifDirectoryAssetQuery : FileStructureAssetQuery {
    protected override string QueryName => "Nif";
    protected override bool CacheAssets => true;

    private readonly ModelAssetQuery _modelAssetQuery;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;

    public NifDirectoryAssetQuery(ILifetimeScope lifetimeScope) : base(lifetimeScope) {
        var newScope = lifetimeScope.BeginLifetimeScope().DisposeWith(this);
        _modelAssetQuery = newScope.Resolve<ModelAssetQuery>();
        _dataDirectoryProvider = newScope.Resolve<IDataDirectoryProvider>();
    }

    public override IEnumerable<AssetQueryResult<string>> ParseFile(string file) {
        var dataRelativePath = FileSystem.Path.GetRelativePath(_dataDirectoryProvider.Path, file);

        var type = AssetTypeService.GetAssetType(file);
        if (type == AssetTypeService.Provider.Model) {
            foreach (var result in _modelAssetQuery.ParseAssets(file)) {
                yield return result with { Reference = dataRelativePath };
            }
        }
    }
}
