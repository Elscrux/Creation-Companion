using System.Reactive;
using CreationEditor.Core;
using CreationEditor.Services.Query.Select;
using DynamicData.Binding;
using Noggog;
namespace CreationEditor.Services.Query.Where;

public sealed record QueryConditionMemento(
    object? CompareValue,
    QueryConditionMemento[] SubConditions,
    QueryFieldSelectorMemento FieldSelector,
    string SelectedFunctionOperator,
    bool IsOr,
    bool Negate);

public interface IQueryCondition : IMementoProvider<QueryConditionMemento>, IMementoReceiver<QueryConditionMemento>, IDisposableDropoff {
    IObservableCollection<IQueryCompareFunction> CompareFunctions { get; }
    IQueryCompareFunction? SelectedCompareFunction { get; }

    QueryConditionState ConditionState { get; set; }

    IQueryFieldSelector FieldSelector { get; }

    bool IsOr { get; set; }
    bool Negate { get; set; }

    IObservable<Unit> ConditionChanged { get; }
    IObservable<string> Summary { get; }

    bool Evaluate(object? obj);
}
