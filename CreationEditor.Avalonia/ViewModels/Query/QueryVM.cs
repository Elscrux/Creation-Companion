using CreationEditor.Services.Environment;
using CreationEditor.Services.Query;
using CreationEditor.Services.Query.Where;
using Mutagen.Bethesda.Plugins.Cache;
namespace CreationEditor.Avalonia.ViewModels.Query;

public sealed class QueryVM : ViewModel {
    private readonly ILinkCacheProvider _linkCacheProvider;

    public IQueryRunner QueryRunner { get; }
    public IQueryConditionFactory ConditionFactory { get; }
    public IObservable<ILinkCache> LinkCacheChanged => _linkCacheProvider.LinkCacheChanged;

    public QueryVM(
        ILinkCacheProvider linkCacheProvider,
        IQueryRunner queryRunner,
        IQueryConditionFactory conditionFactory) {
        _linkCacheProvider = linkCacheProvider;
        QueryRunner = queryRunner;
        ConditionFactory = conditionFactory;
    }
}
