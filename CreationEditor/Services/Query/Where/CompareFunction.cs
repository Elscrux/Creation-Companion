namespace CreationEditor.Services.Query.Where;

public sealed record CompareFunction<TField, TCompareValue> : ICompareFunction {
    private readonly Func<ConditionState, TField, bool> _evaluate;
    private readonly Func<Type, IEnumerable<FieldType>> _getFields;

    public string Operator { get; }

    /// <summary>
    /// Create a new <see cref="CompareFunction{TField,TCompareValue}"/>.
    /// </summary>
    /// <param name="operator">Operator Name</param>
    /// <param name="evaluate">Used to evaluate the function</param>
    /// <param name="fieldCategory">Determines the fields that should be used by default</param>
    /// <exception cref="ArgumentOutOfRangeException">Invalid field category</exception>
    public CompareFunction(
        string @operator,
        Func<ConditionState, TField, bool> evaluate,
        FieldCategory fieldCategory = FieldCategory.Value) {
        Operator = @operator;
        _evaluate = evaluate;
        _getFields = fieldCategory switch {
            FieldCategory.Value => ValueFields,
            FieldCategory.Collection => CollectionFields,
            _ => throw new ArgumentOutOfRangeException(nameof(fieldCategory), fieldCategory, null)
        };
    }

    /// <summary>
    /// Create a new <see cref="CompareFunction{TField,TCompareValue}"/>.
    /// </summary>
    /// <param name="operator">Operator Name</param>
    /// <param name="evaluate">Used to evaluate the function</param>
    /// <param name="getFields">Custom function to determine which fields should be used</param>
    /// <exception cref="ArgumentOutOfRangeException">Invalid field category</exception>
    public CompareFunction(
        string @operator,
        Func<ConditionState, TField, bool> evaluate,
        Func<Type, IEnumerable<FieldType>> getFields) {
        Operator = @operator;
        _evaluate = evaluate;
        _getFields = getFields;
    }

    /// <inheritdoc/>
    /// <summary>
    /// Create a new <see cref="T:CreationEditor.Services.Query.Where.CompareFunction`2" />.
    /// Convenience constructor for compare functions on value level.
    /// Automatically takes care that the value uses the right type.
    /// </summary>
    /// <param name="operator">Operator Name</param>
    /// <param name="evaluate">Used to evaluate the function</param>
    public CompareFunction(string @operator, Func<TField, TCompareValue, bool> evaluate)
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

    public IEnumerable<FieldType> GetFields(Type actualType) => _getFields(actualType);

    public bool Evaluate(ConditionState conditionState, object fieldValue) {
        if (fieldValue is TField fieldT) {
            return _evaluate(conditionState, fieldT);
        }
        return false;
    }

    private static IEnumerable<FieldType> ValueFields(Type type) {
        yield return new FieldType(typeof(TCompareValue), type, FieldCategory.Value);
    }
    private static IEnumerable<FieldType> CollectionFields(Type type) {
        var listType = type.GetGenericArguments().FirstOrDefault() ?? type;
        yield return new FieldType(listType, listType, FieldCategory.Collection);
    }
}
