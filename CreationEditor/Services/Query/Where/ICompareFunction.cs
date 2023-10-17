namespace CreationEditor.Services.Query.Where;

public interface ICompareFunction {
    string Operator { get; }
    IEnumerable<FieldType> GetFields(Type actualType);

    bool Evaluate(ConditionState conditionState, object fieldValue);
}
