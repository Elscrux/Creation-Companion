namespace CreationEditor.Services.Query.Where;

public interface ICompareFunction {
    string Operator { get; }
    IFieldInformation? GetField(Type actualType);

    bool Evaluate(ConditionState conditionState, object fieldValue);
}
