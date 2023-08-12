using System.Reactive;
using System.Reactive.Linq;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query;

public interface IQueryConditionEntry {
    IRecordFieldSelector RecordFieldSelector { get; }
    IQueryCondition Condition { get; }

    bool IsOr { get; set; }
    bool Negate { get; set; }

    IObservable<Unit> ConditionEntryChanged { get; }
    IObservable<string> Summary { get; }

    bool Evaluate(object? obj);
}

public sealed class QueryConditionEntry : ReactiveObject, IQueryConditionEntry, IDisposable {
    private readonly IDisposableDropoff _disposables = new DisposableBucket();

    public IRecordFieldSelector RecordFieldSelector { get; }
    [Reactive] public IQueryCondition Condition { get; private set; } = new NullCondition();
    [Reactive] public bool IsOr { get; set; }
    [Reactive] public bool Negate { get; set; }

    public IObservable<Unit> ConditionEntryChanged { get; }
    public IObservable<string> Summary { get; }

    public QueryConditionEntry(
        IQueryConditionProvider queryConditionProvider,
        IRecordFieldSelector recordFieldSelector,
        Type? recordType = null) {
        RecordFieldSelector = recordFieldSelector;
        RecordFieldSelector.RecordType = recordType;

        Summary = this.WhenAnyValue(
                x => x.RecordFieldSelector.SelectedField,
                x => x.Negate,
                x => x.IsOr,
                (field, negate, or) => (Field: field, Negate: negate, Or: or))
            .CombineLatest(this.WhenAnyValue(x => x.Condition)
                    .Select(c => c.Summary)
                    .Switch(),
                (x, condition) =>
                    x.Field?.Name
                  + (x.Negate ? " Not " : " ")
                  + condition
                  + (x.Or ? " Or " : " And "));

        var conditionChanged = this.WhenAnyValue(x => x.Condition)
            .Select(condition => {
                return condition switch {
                    IQueryListCondition queryListCondition => queryListCondition.SubConditions
                        .ObserveCollectionChanges()
                        .Select(_ => queryListCondition.SubConditions
                            .Select(x => x.ConditionEntryChanged)
                            .Merge())
                        .Switch(),
                    IQueryValueCondition queryValueCondition => queryValueCondition.WhenAnyValue(x => x.CompareValue).Unit(),
                    _ => throw new ArgumentOutOfRangeException(nameof(condition))
                };
            })
            .Switch();

        ConditionEntryChanged = this.WhenAnyValue(
                x => x.Condition,
                x => x.Condition.SelectedFunction,
                x => x.Negate,
                x => x.IsOr,
                x => x.RecordFieldSelector.SelectedField)
            .Unit()
            .Merge(conditionChanged);

        this.WhenAnyValue(x => x.RecordFieldSelector.SelectedField)
            .NotNull()
            .Subscribe(field => Condition = queryConditionProvider.GetCondition(field.Type))
            .DisposeWith(_disposables);
    }

    public bool Evaluate(object? obj) {
        if (Condition is null || RecordFieldSelector.SelectedField is null) return false;

        var fieldValue = RecordFieldSelector.SelectedField.GetValue(obj);
        if (fieldValue is null) return false;

        return Condition.Evaluate(fieldValue);
    }

    public void Dispose() => _disposables.Dispose();
}

public static class QueryConditionExtensions {
    public static bool EvaluateConditions(this IEnumerable<IQueryConditionEntry> conditions, object? obj) {
        if (obj is null) return false;

        var andStack = new Stack<bool>();

        foreach (var condition in conditions) {
            // Evaluate condition result
            var result = condition.Evaluate(obj);
            if (condition.Negate) result = !result;

            // Push current condition result to stack
            andStack.Push(result);

            // If condition is an OR, check if all conditions in the stack are true
            if (condition.IsOr && ValidateAndStack()) return true;
        }

        return andStack.Count > 0 && ValidateAndStack();

        bool ValidateAndStack() {
            // Check if all conditions in the stack are true
            // If so, we have a valid and block in our or block, so return true
            if (andStack.All(x => x)) return true;

            // Otherwise, clear the AND stack and continue
            andStack.Clear();
            return false;
        }
    }
}
