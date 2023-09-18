using System.Reactive;
using CreationEditor.Core;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Services.Query.Where;

public abstract class QueryValueCondition<TField, TCompareValue> : ReactiveObject, IQueryValueCondition, IMementoProvider<QueryValueConditionMemento>
    where TField : notnull {
    public virtual int Priority => 0;
    public Type UnderlyingType { get; set; } = typeof(TField);

    private readonly IList<CompareFunction<TField, TCompareValue>> _functions;
    IList<ICompareFunction> IQueryCondition.Functions => _functions.Cast<ICompareFunction>().ToArray();

    private CompareFunction<TField, TCompareValue> _selectedFunction;
    public ICompareFunction SelectedFunction {
        get => _selectedFunction;
        set {
            if (value is CompareFunction<TField, TCompareValue> function) {
                this.RaiseAndSetIfChanged(ref _selectedFunction, function);
            }
        }
    }

    private TCompareValue? _compareValue;
    public object? CompareValue {
        get => _compareValue;
        set {
            if (value is TCompareValue field) {
                this.RaiseAndSetIfChanged(ref _compareValue, field);
            } else if (value is IConvertible) {
                if (Convert.ChangeType(value, typeof(TCompareValue)) is TCompareValue converted) {
                    this.RaiseAndSetIfChanged(ref _compareValue, converted);
                }
            }
        }
    }

    public IObservable<string> Summary { get; }
    public IObservable<Unit> ConditionChanged { get; }

    protected QueryValueCondition(IList<CompareFunction<TField, TCompareValue>> functions) {
        _functions = functions;
        SelectedFunction = _selectedFunction = _functions.First();

        Summary = this.WhenAnyValue(
            x => x.SelectedFunction,
            x => x.CompareValue,
            (function, value) => function.Operator + " " + value);

        ConditionChanged = this.WhenAnyValue(x => x.CompareValue).Unit();
    }

    public override string ToString() {
        return SelectedFunction.Operator + " " + CompareValue;
    }

    public virtual bool Accepts(Type type) => type.InheritsFrom(typeof(TField));
    public bool Evaluate(object? fieldValue) {
        if (CompareValue is null || fieldValue is not TField field) return false;

        return SelectedFunction.Evaluate(field, CompareValue);
    }

    public virtual List<FieldType> GetFields() {
        const string compareValueName = nameof(CompareValue);
        return new List<FieldType> { new(typeof(TField), UnderlyingType, compareValueName) };
    }

    public QueryValueConditionMemento CreateMemento() {
        return new QueryValueConditionMemento(
            UnderlyingType.AssemblyQualifiedName ?? string.Empty,
            SelectedFunction.Operator,
            CompareValue);
    }
    public void RestoreMemento(QueryValueConditionMemento memento) {
        UnderlyingType = Type.GetType(memento.FullTypeName) ?? typeof(TField);
        SelectedFunction = _functions.FirstOrDefault(x => x.Operator == memento.SelectedFunctionOperator) ?? _functions.First();
        CompareValue = memento.CompareValue;
    }

    IQueryConditionMemento IMementoProvider<IQueryConditionMemento>.CreateMemento() => CreateMemento();
    public void RestoreMemento(IQueryConditionMemento memento) {
        if (memento is QueryValueConditionMemento valueDto) {
            RestoreMemento(valueDto);
        }
    }
}
