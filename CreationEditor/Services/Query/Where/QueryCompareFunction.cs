namespace CreationEditor.Services.Query.Where;

public sealed record QueryCompareFunction<TField, TCompareValue> : IQueryCompareFunction {
    private readonly Func<QueryConditionState, TField, bool> _evaluate;
    private readonly Func<Type, IQueryFieldInformation> _getField;

    public string Operator { get; }

    /// <summary>
    /// Create a new <see cref="QueryCompareFunction{TField,TCompareValue}"/>.
    /// </summary>
    /// <param name="operator">Operator Name</param>
    /// <param name="evaluate">Used to evaluate the function</param>
    /// <param name="queryFieldCategory">Determines the field category which is used to edit the set value</param>
    /// <exception cref="ArgumentOutOfRangeException">Invalid field category</exception>
    public QueryCompareFunction(
        string @operator,
        Func<QueryConditionState, TField, bool> evaluate,
        QueryFieldCategory queryFieldCategory = QueryFieldCategory.Value) {
        Operator = @operator;
        _evaluate = evaluate;
        _getField = queryFieldCategory switch {
            QueryFieldCategory.Value => ValueFields,
            QueryFieldCategory.Collection => CollectionFields,
            _ => throw new ArgumentOutOfRangeException(nameof(queryFieldCategory), queryFieldCategory, null)
        };
    }

    /// <summary>
    /// Create a new <see cref="QueryCompareFunction{TField,TCompareValue}"/>.
    /// </summary>
    /// <param name="operator">Operator Name</param>
    /// <param name="evaluate">Used to evaluate the function</param>
    /// <param name="getField">Custom function to determine which field should be used</param>
    /// <exception cref="ArgumentOutOfRangeException">Invalid field category</exception>
    public QueryCompareFunction(
        string @operator,
        Func<QueryConditionState, TField, bool> evaluate,
        Func<Type, IQueryFieldInformation> getField) {
        Operator = @operator;
        _evaluate = evaluate;
        _getField = getField;
    }

    /// <inheritdoc/>
    /// <summary>
    /// Create a new <see cref="T:CreationEditor.Services.Query.Where.CompareFunction`2" />.
    /// Convenience constructor for compare functions on value level.
    /// Automatically takes care that the value uses the right type.
    /// </summary>
    /// <param name="operator">Operator Name</param>
    /// <param name="evaluate">Used to evaluate the function</param>
    public QueryCompareFunction(string @operator, Func<TField, TCompareValue, bool> evaluate)
        : this(@operator, (state, fieldValue) => {
            switch (state.CompareValue) {
                case TCompareValue compareValue:
                    return evaluate(fieldValue, compareValue);
                case null:
                    return fieldValue is null;
                default:
                    var x = (TCompareValue?) Convert.ChangeType(state.CompareValue, typeof(TCompareValue));
                    return x is not null && evaluate(fieldValue, x);
            }
        }) {}

    public IQueryFieldInformation GetField(Type actualType) => _getField(actualType);

    public bool Evaluate(QueryConditionState conditionState, object fieldValue) {
        if (fieldValue is TField fieldT) {
            return _evaluate(conditionState, fieldT);
        }
        return false;
    }

    private static ValueQueryFieldInformation ValueFields(Type type) {
        return new ValueQueryFieldInformation(typeof(TCompareValue), type);
    }
    private static CollectionQueryFieldInformation CollectionFields(Type type) {
        var listType = type.GetGenericArguments().FirstOrDefault() ?? type;
        return new CollectionQueryFieldInformation(listType, listType);
    }
}
