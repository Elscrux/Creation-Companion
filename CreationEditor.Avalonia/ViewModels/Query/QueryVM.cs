using CreationEditor.Services.Environment;
using CreationEditor.Services.Query;
using CreationEditor.Services.Query.Where;
using Mutagen.Bethesda.Plugins.Cache;
namespace CreationEditor.Avalonia.ViewModels.Query;

public sealed class QueryVM(
        ILinkCacheProvider linkCacheProvider,
        IQueryRunner queryRunner,
        IQueryConditionFactory conditionFactory)
    : ViewModel {

    public IQueryRunner QueryRunner { get; } = queryRunner;
    public IQueryConditionFactory ConditionFactory { get; } = conditionFactory;
    public IObservable<ILinkCache> LinkCacheChanged { get; } = linkCacheProvider.LinkCacheChanged;
}
