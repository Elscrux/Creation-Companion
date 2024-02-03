using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Services.Query.Select;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query.Where;

public sealed class QueryCondition : ReactiveObject, IQueryCondition {
    private readonly IQueryConditionFactory _queryConditionFactory;
    private readonly DisposableBucket _disposables = new();

    [Reactive] public ConditionState ConditionState { get; set; }
    public IFieldSelector FieldSelector { get; } = new ReflectionFieldSelector();

    public IObservableCollection<ICompareFunction> CompareFunctions { get; } = new ObservableCollectionExtended<ICompareFunction>();
    [Reactive] public ICompareFunction? SelectedCompareFunction { get; set; }

    public IObservableCollection<IQueryCondition> SubConditions => ConditionState.SubConditions;
    public object? CompareValue {
        get => ConditionState.CompareValue;
        set => ConditionState.CompareValue = value;
    }

    [Reactive] public bool IsOr { get; set; }
    [Reactive] public bool Negate { get; set; }

    public IObservable<Unit> ConditionChanged { get; }
    public IObservable<string> Summary { get; }

    public QueryCondition(
        IQueryConditionFactory queryConditionFactory,
        ICompareFunctionFactory compareFunctionFactory,
        Type? recordType = null) {
        _queryConditionFactory = queryConditionFactory;
        FieldSelector.RecordType = recordType;
        ConditionState = new ConditionState(SelectedCompareFunction, FieldSelector.SelectedField?.Type, _queryConditionFactory);

        var conditionStateChanges = this.WhenAnyValue(x => x.ConditionState);

        var subConditionsChanged = conditionStateChanges
            .Select(s => s.SubConditions.SelectWhenCollectionChanges(
                () => s.SubConditions
                    .Select(x => x.ConditionChanged)
                    .Merge()))
            .Switch();

        var functionChanged = this.WhenAnyValue(x => x.SelectedCompareFunction);

        var conditionChanged = this.WhenAnyValue(x => x.ConditionState.CompareValue).Unit()
            .Merge(functionChanged.Unit())
            .Merge(subConditionsChanged);

        ConditionChanged = this.WhenAnyValue(
                x => x.SelectedCompareFunction,
                x => x.Negate,
                x => x.IsOr,
                x => x.FieldSelector.SelectedField)
            .Unit()
            .Merge(conditionChanged);

        this.WhenAnyValue(x => x.FieldSelector.SelectedField)
            .NotNull()
            .Subscribe(field => {
                CompareFunctions.ReplaceWith(compareFunctionFactory.Get(field.Type));
                SelectedCompareFunction = CompareFunctions.FirstOrDefault();
                ConditionState = new ConditionState(SelectedCompareFunction, field.Type, _queryConditionFactory);
            })
            .DisposeWith(_disposables);

        functionChanged
            .Pairwise()
            .Subscribe(function => {
                if (FieldSelector.SelectedField is null) return;

                // Don't update if the fields are the same
                if (function.Previous is not null && function.Current is not null) {
                    var old = function.Previous.GetField(FieldSelector.SelectedField.Type);
                    var updated = function.Current.GetField(FieldSelector.SelectedField.Type);
                    if (Equals(old, updated)) return;
                }

                ConditionState = new ConditionState(function.Current, FieldSelector.SelectedField?.Type, _queryConditionFactory);
            })
            .DisposeWith(this);

        Summary = this.WhenAnyValue(
                x => x.FieldSelector.SelectedField,
                x => x.Negate,
                x => x.IsOr,
                x => x.SelectedCompareFunction,
                x => x.ConditionState.CompareValue,
                (field, negate, or, function, compareValue) => (Field: field, Negate: negate, Or: or, Function: function, CompareValue: compareValue))
            .CombineLatest(conditionStateChanges
                    .Select(s => s.SubConditions.SelectWhenCollectionChanges(() => s.Summary.Select(x => (Summary: x, State: s))))
                    .Switch(),
                (condition, x) => (Condition: condition, x.Summary, x.State))
            .Select(x =>
                x.Condition.Field?.Name
              + (x.Condition.Negate ? " Not " : " ")
              + (x.Condition.Function is null
                    ? string.Empty
                    : $"{x.Condition.Function.Operator} {(x.Summary.Count > 0
                        ? $"({x.State.GetFullSummary(x.Summary)})"
                        : x.Condition.CompareValue)}"));
    }

    public bool Evaluate(object? obj) {
        if (SelectedCompareFunction is null
         || FieldSelector.SelectedField is null) return false;

        var fieldValue = FieldSelector.SelectedField.GetValue(obj);
        if (fieldValue is null) return false;

        return SelectedCompareFunction.Evaluate(ConditionState, fieldValue);
    }

    public QueryConditionMemento CreateMemento() {
        return new QueryConditionMemento(
            CompareValue,
            SubConditions.Select(c => c.CreateMemento()).ToArray(),
            FieldSelector.CreateMemento(),
            SelectedCompareFunction?.Operator ?? string.Empty,
            IsOr,
            Negate);
    }

    public void RestoreMemento(QueryConditionMemento memento) {
        // Field
        FieldSelector.RestoreMemento(memento.FieldSelector);
        SelectedCompareFunction = CompareFunctions.FirstOrDefault(x => x.Operator == memento.SelectedFunctionOperator) ?? CompareFunctions.FirstOrDefault();

        // Function and Compare Value
        ConditionState = new ConditionState(SelectedCompareFunction, FieldSelector.SelectedField?.Type);
        CompareValue = memento.CompareValue;
        SubConditions.AddRange(memento.SubConditions.Select(x => {
            var queryCondition = _queryConditionFactory.Create();
            queryCondition.RestoreMemento(x);
            return queryCondition;
        }));

        // Flags
        IsOr = memento.IsOr;
        Negate = memento.Negate;
    }

    public void Dispose() => _disposables.Dispose();
    public void Add(IDisposable disposable) => _disposables.Add(disposable);
}
