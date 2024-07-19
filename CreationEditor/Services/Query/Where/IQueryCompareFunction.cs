namespace CreationEditor.Services.Query.Where;

public interface IQueryCompareFunction {
    string Operator { get; }
    IQueryFieldInformation? GetField(Type actualType);

    bool Evaluate(QueryConditionState conditionState, object fieldValue);
}
