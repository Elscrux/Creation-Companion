using System.Reactive;
using CreationEditor.Core;
using CreationEditor.Services.Query.Select;
namespace CreationEditor.Services.Query.Where;

public sealed record QueryConditionEntryMemento(
    FieldSelectorMemento FieldSelector,
    IQueryConditionMemento QueryCondition,
    bool IsOr,
    bool Negate);

public interface IQueryConditionEntry : IMementoProvider<QueryConditionEntryMemento> {
    IFieldSelector FieldSelector { get; }
    IQueryCondition Condition { get; }

    bool IsOr { get; set; }
    bool Negate { get; set; }

    IObservable<Unit> ConditionEntryChanged { get; }
    IObservable<string> Summary { get; }

    bool Evaluate(object? obj);
}
