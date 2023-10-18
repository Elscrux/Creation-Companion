namespace CreationEditor.Services.Query.Where;

public sealed class QueryConditionFactory : IQueryConditionFactory {
    private readonly Func<Type?, IQueryCondition> _typeQueryConditionEntryFactory;

    public QueryConditionFactory(
        Func<Type?, IQueryCondition> typeQueryConditionEntryFactory) {
        _typeQueryConditionEntryFactory = typeQueryConditionEntryFactory;
    }

    public IQueryCondition Create(Type? type = null) {
        return _typeQueryConditionEntryFactory(type);
    }
}
