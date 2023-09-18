using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Core;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Services.Query.Where;

public abstract class QueryListCondition<TField, TValue> : ReactiveObject, IQueryListCondition, IMementoProvider<QueryListConditionMemento>
    where TField : notnull {
    public virtual int Priority => 0;
    public Type FieldTypeClass => typeof(TField);
    public Type UnderlyingType { get; set; } = typeof(TField);
    public Type CompareValueType => typeof(TValue);

    private readonly IQueryConditionEntryFactory _queryConditionEntryFactory;
    private readonly IList<CompareFunction<TField, TValue>> _functions;
    IList<ICompareFunction> IQueryCondition.Functions => _functions.Cast<ICompareFunction>().ToArray();

    private CompareFunction<TField, TValue> _selectedFunction;
    public ICompareFunction SelectedFunction {
        get => _selectedFunction;
        set {
            if (value is CompareFunction<TField, TValue> function) {
                this.RaiseAndSetIfChanged(ref _selectedFunction, function);
            }
        }
    }

    public IObservableCollection<IQueryConditionEntry> SubConditions { get; } = new ObservableCollectionExtended<IQueryConditionEntry>();
    public IObservable<string> Summary { get; }
    public IObservable<Unit> ConditionChanged { get; }

    protected QueryListCondition(
        IQueryConditionEntryFactory queryConditionEntryFactory,
        IList<CompareFunction<TField, TValue>> functions) {
        _queryConditionEntryFactory = queryConditionEntryFactory;
        _functions = functions;
        SelectedFunction = _selectedFunction = _functions.First();

        Summary = SubConditions.Select(x => x.Summary).CombineLatest()
            .CombineLatest(this.WhenAnyValue(x => x.SelectedFunction),
                (list, function) => function.Operator + " " + string.Join(' ', list));

        ConditionChanged = SubConditions
            .ObserveCollectionChanges()
            .Select(_ => SubConditions
                .Select(x => x.ConditionEntryChanged)
                .Merge())
            .Switch();
    }

    public override string ToString() {
        return SelectedFunction.Operator + " "
          + string.Join(' ', SubConditions.Select(x => x.ToString()));
    }

    public virtual bool Accepts(Type type) => type.InheritsFrom(FieldTypeClass);
    public abstract bool Evaluate(object? fieldValue);

    public virtual List<FieldType> GetFields() {
        var listType = UnderlyingType.GetGenericArguments().FirstOrDefault() ?? UnderlyingType;
        const string subConditionsName = nameof(SubConditions);
        return new List<FieldType> { new(listType, listType, subConditionsName) };
    }

    public QueryListConditionMemento CreateMemento() {
        return new QueryListConditionMemento(
            UnderlyingType.AssemblyQualifiedName ?? string.Empty,
            SelectedFunction.Operator,
            SubConditions.Select(x => x.CreateMemento()).ToList());
    }
    public void RestoreMemento(QueryListConditionMemento memento) {
        UnderlyingType = Type.GetType(memento.FullTypeName) ?? typeof(TField);
        SelectedFunction = _functions.FirstOrDefault(x => x.Operator == memento.SelectedFunctionOperator) ?? _functions.First();
        SubConditions.Clear();
        SubConditions.AddRange(memento.SubConditions.Select(x => {
            var queryConditionEntry = _queryConditionEntryFactory.Create(UnderlyingType);
            queryConditionEntry.RestoreMemento(x);
            return queryConditionEntry;
        }));
    }

    IQueryConditionMemento IMementoProvider<IQueryConditionMemento>.CreateMemento() => CreateMemento();
    public void RestoreMemento(IQueryConditionMemento memento) {
        if (memento is QueryListConditionMemento listDto) {
            RestoreMemento(listDto);
        }
    }
}
