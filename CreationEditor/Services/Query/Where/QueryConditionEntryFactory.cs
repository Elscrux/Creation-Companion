namespace CreationEditor.Services.Query.Where;

public sealed class QueryConditionEntryFactory : IQueryConditionEntryFactory {
    private readonly Func<Type?, IQueryConditionEntry> _typeQueryConditionEntryFactory;

    public QueryConditionEntryFactory(
        Func<Type?, IQueryConditionEntry> typeQueryConditionEntryFactory) {
        _typeQueryConditionEntryFactory = typeQueryConditionEntryFactory;
    }

    public IQueryConditionEntry Create(Type? type = null) {
        return _typeQueryConditionEntryFactory(type);
    }
}
