﻿using System.Reactive.Linq;
using System.Text;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query.Where;

public sealed class ConditionState : ReactiveObject {
    private readonly ICompareFunction? _selectedCompareFunction;
    private readonly Type? _underlyingType;

    [Reactive] public object? CompareValue { get; set; }
    public IObservableCollection<IQueryCondition> SubConditions { get; } = new ObservableCollectionExtended<IQueryCondition>();

    /// <summary>
    /// Create a new condition state.
    /// </summary>
    /// <param name="selectedCompareFunction">Condition compare function</param>
    /// <param name="underlyingType">Type of field</param>
    public ConditionState(
        ICompareFunction? selectedCompareFunction,
        Type? underlyingType) {
        _selectedCompareFunction = selectedCompareFunction;
        _underlyingType = underlyingType;
    }

    /// <inheritdoc />
    /// <param name="queryConditionFactory">Factory to initialize a first field</param>
    public ConditionState(
        ICompareFunction? selectedCompareFunction,
        Type? underlyingType,
        IQueryConditionFactory queryConditionFactory)
        : this(selectedCompareFunction, underlyingType) {
        var fieldInformation = GetField();
        if (fieldInformation is CollectionFieldInformation collection) {
            SubConditions.Add(queryConditionFactory.Create(collection.ElementType));
        }
    }

    public IFieldInformation? GetField() {
        if (_underlyingType is null || _selectedCompareFunction is null) return null;

        return _selectedCompareFunction.GetField(_underlyingType);
    }

    public IObservable<IList<string>> Summary => SubConditions
        .Select(x => x.Summary)
        .CombineLatest()
        .StartWith(Array.Empty<string>());

    public string GetFullSummary(IList<string> summaries) {
        if (summaries.Count > SubConditions.Count) return string.Empty;

        var sb = new StringBuilder();
        for (var i = 0; i < summaries.Count - 1; i++) {
            sb.Append(summaries[i]);
            sb.Append(SubConditions[i].IsOr ? " Or " : " And ");
        }
        sb.Append(summaries[^1]);
        return sb.ToString();
    }
}
