using System.Reactive;
using CreationEditor.Core;
using CreationEditor.Services.Query.Select;
using DynamicData.Binding;
using Noggog;
namespace CreationEditor.Services.Query.Where;

public sealed record QueryConditionMemento(
    object? CompareValue,
    QueryConditionMemento[] SubConditions,
    FieldSelectorMemento FieldSelector,
    string SelectedFunctionOperator,
    bool IsOr,
    bool Negate);

public interface IQueryCondition : IMementoProvider<QueryConditionMemento>, IDisposableDropoff {
    IObservableCollection<ICompareFunction> CompareFunctions { get; }
    ICompareFunction? SelectedCompareFunction { get; }

    ConditionState ConditionState { get; set; }

    IFieldSelector FieldSelector { get; }

    bool IsOr { get; set; }
    bool Negate { get; set; }

    IObservable<Unit> ConditionChanged { get; }
    IObservable<string> Summary { get; }

    bool Evaluate(object? obj);
}
