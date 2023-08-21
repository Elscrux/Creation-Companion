using Noggog;
namespace CreationEditor.Services.Query.Where;

public sealed class QueryConditionFactory : IQueryConditionFactory {
    private readonly Func<Type, IQueryCondition?> _queryConditionFactory;
    private readonly IQueryCondition[] _conditionCache;

    public QueryConditionFactory(
        IEnumerable<IQueryCondition> queryConditions,
        Func<Type, IQueryCondition?> queryConditionFactory) {
        _queryConditionFactory = queryConditionFactory;
        _conditionCache = queryConditions
            .Where(function => function is not NullValueCondition)
            .ToArray();
    }

    public IQueryCondition Create(Type type) {
        if (type.InheritsFrom(typeof(ReadOnlyMemorySlice<>))) return new NullValueCondition();

        type = type.InheritsFrom(typeof(Nullable<>))
            ? type.GetGenericArguments()[0]
            : type;

        var conditionType = _conditionCache
            .Where(x => x.Accepts(type))
            .MaxBy(x => x.Priority)?
            .GetType();

        if (conditionType is not null && _queryConditionFactory(conditionType) is {} condition) {
            condition.ActualFieldType = type;
            return condition;
        }

        return new NullValueCondition();
    }
}
