using Autofac;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Query;
using Mutagen.Bethesda.Plugins.Cache;
namespace CreationEditor.Avalonia.ViewModels.Query;

public sealed class QueryVM : ViewModel {
    private readonly IEditorEnvironment _editorEnvironment;

    public ILifetimeScope LifetimeScope { get; }
    public IQueryRunner QueryRunner { get; set; }
    public IObservable<ILinkCache> LinkCacheChanged => _editorEnvironment.LinkCacheChanged;

    public QueryVM(
        IEditorEnvironment editorEnvironment,
        ILifetimeScope lifetimeScope) {
        _editorEnvironment = editorEnvironment;
        LifetimeScope = lifetimeScope;

        var newScope = lifetimeScope.BeginLifetimeScope();
        var queryFrom = newScope.Resolve<QueryFromRecordType>();
        QueryRunner = newScope.Resolve<IQueryRunner>(TypedParameter.From<IQueryFrom>(queryFrom));
    }
}
