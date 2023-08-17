namespace CreationEditor.Services.Query.Where;

public sealed record QueryConditionValueMemento(
    string FullTypeName,
    string SelectedFunctionOperator,
    object? CompareValue) : IQueryConditionMemento;

public interface IQueryValueCondition : IQueryCondition {
    object? CompareValue { get; set; }
}
