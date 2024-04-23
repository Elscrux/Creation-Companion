namespace CreationEditor.Services.Query.Where;

public sealed class QueryConditionFactory(Func<Type?, IQueryCondition> typeQueryConditionFactory) : IQueryConditionFactory {
    public IQueryCondition Create(Type? type = null) {
        return typeQueryConditionFactory(type);
    }
}
