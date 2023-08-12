using Autofac;
using Noggog;
namespace CreationEditor.Services.Query;

public sealed class QueryConditionProvider : IQueryConditionProvider {
    private readonly IQueryCondition[] _functionCache;

    public QueryConditionProvider() {
        _functionCache = typeof(IQueryCondition)
            .GetAllSubClasses<IQueryCondition>()
            .Where(function => function is not NullCondition)
            .ToArray();
    }

    public IQueryCondition GetCondition(Type type) {
        if (type.InheritsFrom(typeof(ReadOnlyMemorySlice<>))) return new NullCondition();

        type = type.InheritsFrom(typeof(Nullable<>))
            ? type.GetGenericArguments()[0]
            : type;

        var functionType = _functionCache
            .Where(x => x.Accepts(type))
            .MaxBy(x => x.Priority)?
            .GetType();

        if (functionType is not null && Activator.CreateInstance(functionType) is IQueryCondition condition) {
            condition.ActualFieldType = type;
            return condition;
        }

        return new NullCondition();
    }
}
