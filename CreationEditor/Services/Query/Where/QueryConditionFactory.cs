namespace CreationEditor.Services.Query.Where;

public sealed class QueryConditionFactory : IQueryConditionFactory {
    private readonly Func<Type?, IQueryCondition> _typeQueryConditionFactory;

    public QueryConditionFactory(
        Func<Type?, IQueryCondition> typeQueryConditionFactory) {
        _typeQueryConditionFactory = typeQueryConditionFactory;
    }

    public IQueryCondition Create(Type? type = null) {
        return _typeQueryConditionFactory(type);
    }
}
