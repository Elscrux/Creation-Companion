﻿using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Services.Query.Select;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query.Where;

public sealed class QueryCondition : ReactiveObject, IQueryCondition {
    private readonly IQueryConditionFactory _queryConditionFactory;
    private readonly IDisposableDropoff _disposables = new DisposableBucket();

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
        ConditionState = new ConditionState(SelectedCompareFunction, FieldSelector.SelectedField?.Type);

        var subConditionsChanged = this.WhenAnyValue(x => x.ConditionState)
            .Select(x => x.SubConditions.ObserveCollectionChanges())
            .Switch()
            .Select(_ => ConditionState.SubConditions
                .Select(x => x.ConditionChanged)
                .Merge())
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
                CompareFunctions.Clear();
                CompareFunctions.AddRange(compareFunctionFactory.Get(field.Type));
                SelectedCompareFunction = CompareFunctions.FirstOrDefault();
                ConditionState = new ConditionState(SelectedCompareFunction, field.Type);
            })
            .DisposeWith(_disposables);

        functionChanged
            .Pairwise()
            .Subscribe(function => {
                if (FieldSelector.SelectedField is null) return;

                // Don't update if the fields are the same
                if (function.Previous is not null && function.Current is not null) {
                    var old = function.Previous.GetFields(FieldSelector.SelectedField.Type);
                    var updated = function.Current.GetFields(FieldSelector.SelectedField.Type);
                    if (old.SequenceEqual(updated)) return;
                }

                ConditionState = new ConditionState(function.Current, FieldSelector.SelectedField?.Type);
            })
            .DisposeWith(this);

        Summary = this.WhenAnyValue(
                x => x.FieldSelector.SelectedField,
                x => x.Negate,
                x => x.IsOr,
                x => x.SelectedCompareFunction,
                x => x.ConditionState.CompareValue,
                (field, negate, or, function, compareValue) => (Field: field, Negate: negate, Or: or, Function: function, CompareValue: compareValue))
            .CombineLatest(this.WhenAnyValue(x => x.ConditionState)
                    .Select(x => x.SubConditions.ObserveCollectionChanges())
                    .Switch()
                    .Select(_ => ConditionState.SubConditions
                        .Select(x => x.Summary)
                        .Merge())
                    .Switch(),
                (x, condition) =>
                    x.Field?.Name
                  + (x.Negate ? " Not " : " ")
                  + (x.Function is null
                        ? string.Empty
                        : x.CompareValue is null
                            // Sub Conditions
                            ? condition
                            // Compare Value
                            : x.Function?.Operator + " " + x.CompareValue)
                  + (x.Or ? " Or " : " And "));
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