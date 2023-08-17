using Autofac;
namespace CreationEditor.Services.Query.Where;

public sealed class QueryConditionEntryFactory : IQueryConditionEntryFactory {
    private readonly IComponentContext _componentContext;

    public QueryConditionEntryFactory(IComponentContext componentContext) {
        _componentContext = componentContext;
    }

    public IQueryConditionEntry Create(Type? type = null) {
        var typeParameter = TypedParameter.From(type);
        return _componentContext.Resolve<QueryConditionEntry>(typeParameter);
    }
}
