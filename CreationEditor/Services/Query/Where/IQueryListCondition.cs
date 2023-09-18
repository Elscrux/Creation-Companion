using DynamicData.Binding;
namespace CreationEditor.Services.Query.Where;

public sealed record QueryListConditionMemento(
    string FullTypeName,
    string SelectedFunctionOperator,
    List<QueryConditionEntryMemento> SubConditions) : IQueryConditionMemento;

public interface IQueryListCondition : IQueryCondition {
    IObservableCollection<IQueryConditionEntry> SubConditions { get; }
}
