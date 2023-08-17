using Autofac;
using Noggog;
namespace CreationEditor.Services.Query.Where;

public sealed class QueryConditionFactory : IQueryConditionFactory {
    private readonly IComponentContext _componentContext;
    private readonly IQueryCondition[] _conditionCache;

    public QueryConditionFactory(IComponentContext componentContext) {
        _componentContext = componentContext;
        _conditionCache = typeof(IQueryCondition)
            .GetAllSubClasses<IQueryCondition>(_componentContext.Resolve)
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

        if (conditionType is not null && _componentContext.Resolve(conditionType) is IQueryCondition condition) {
            condition.ActualFieldType = type;
            return condition;
        }

        return new NullValueCondition();
    }
}
