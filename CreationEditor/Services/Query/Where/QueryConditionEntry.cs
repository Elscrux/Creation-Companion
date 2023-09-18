using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Services.Query.Select;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query.Where;

public sealed class QueryConditionEntry : ReactiveObject, IQueryConditionEntry, IDisposable {
    private readonly IDisposableDropoff _disposables = new DisposableBucket();

    public IFieldSelector FieldSelector { get; } = new ReflectionFieldSelector();
    [Reactive] public IQueryCondition Condition { get; private set; } = new NullValueCondition();
    [Reactive] public bool IsOr { get; set; }
    [Reactive] public bool Negate { get; set; }

    public IObservable<Unit> ConditionEntryChanged { get; }
    public IObservable<string> Summary { get; }

    public QueryConditionEntry(
        IQueryConditionFactory queryConditionFactory,
        Type? recordType = null) {
        FieldSelector.RecordType = recordType;

        Summary = this.WhenAnyValue(
                x => x.FieldSelector.SelectedField,
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
            .Select(condition => condition.ConditionChanged)
            .Switch();

        ConditionEntryChanged = this.WhenAnyValue(
                x => x.Condition,
                x => x.Condition.SelectedFunction,
                x => x.Negate,
                x => x.IsOr,
                x => x.FieldSelector.SelectedField)
            .Unit()
            .Merge(conditionChanged);

        this.WhenAnyValue(x => x.FieldSelector.SelectedField)
            .NotNull()
            .Subscribe(field => Condition = queryConditionFactory.Create(field.Type))
            .DisposeWith(_disposables);

        // if (dto is not null) {
        //     FieldSelector.SelectedField = dto.FieldSelector.SelectedField;
        //     Condition = queryConditionProvider.GetCondition(dto.Condition);
        //     Condition.SelectedFunction = dto.Condition.SelectedFunctionOperator;
        //     IsOr = dto.IsOr;
        //     Negate = dto.Negate;
        // }
    }

    public bool Evaluate(object? obj) {
        if (Condition is null || FieldSelector.SelectedField is null) return false;

        var fieldValue = FieldSelector.SelectedField.GetValue(obj);
        if (fieldValue is null) return false;

        return Condition.Evaluate(fieldValue);
    }

    public QueryConditionEntryMemento CreateMemento() {
        return new QueryConditionEntryMemento(
            FieldSelector.CreateMemento(),
            Condition.CreateMemento(),
            IsOr,
            Negate);
    }

    public void RestoreMemento(QueryConditionEntryMemento memento) {
        FieldSelector.RestoreMemento(memento.FieldSelector);
        Condition.RestoreMemento(memento.QueryCondition);
        IsOr = memento.IsOr;
        Negate = memento.Negate;
    }

    public void Dispose() => _disposables.Dispose();
}
