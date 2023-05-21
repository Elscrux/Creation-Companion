using Autofac;
using CreationEditor.Services.Environment;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class NifDirectoryAssetQuery : FileStructureAssetQuery {
    protected override string QueryName => "Nif";
    protected override bool CacheAssets => true;

    private readonly ModelAssetQuery _modelAssetQuery;
    private readonly IEnvironmentContext _environmentContext;

    public NifDirectoryAssetQuery(ILifetimeScope lifetimeScope) : base(lifetimeScope) {
        var newScope = lifetimeScope.BeginLifetimeScope().DisposeWith(this);
        _modelAssetQuery = newScope.Resolve<ModelAssetQuery>();
        _environmentContext = newScope.Resolve<IEnvironmentContext>();
    }

    public override IEnumerable<AssetQueryResult<string>> ParseFile(string file) {
        var dataRelativePath = FileSystem.Path.GetRelativePath(_environmentContext.DataDirectoryProvider.Path, file);

        var type = AssetTypeService.GetAssetType(file);
        if (type == AssetTypeService.Provider.Model) {
            foreach (var result in _modelAssetQuery.ParseAssets(file)) {
                yield return result with { Reference = dataRelativePath };
            }
        }
    }
}
