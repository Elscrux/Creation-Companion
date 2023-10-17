using System.Reactive;
using CreationEditor.Core;
using CreationEditor.Services.Query.Select;
using DynamicData.Binding;
using Noggog;
namespace CreationEditor.Services.Query.Where;

public sealed record QueryConditionEntryMemento(
    object? CompareValue,
    QueryConditionEntryMemento[] SubConditions,
    FieldSelectorMemento FieldSelector,
    string SelectedFunctionOperator,
    bool IsOr,
    bool Negate);

public interface IQueryConditionEntry : IMementoProvider<QueryConditionEntryMemento>, IDisposableDropoff {
    IObservableCollection<ICompareFunction> CompareFunctions { get; }
    ICompareFunction? SelectedCompareFunction { get; }

    ConditionState ConditionState { get; set; }

    IFieldSelector FieldSelector { get; }

    bool IsOr { get; set; }
    bool Negate { get; set; }

    IObservable<Unit> ConditionEntryChanged { get; }
    IObservable<string> Summary { get; }

    bool Evaluate(object? obj);
}
