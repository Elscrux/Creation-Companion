using CreationEditor.Core;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Services.Query.Where;

public abstract class QueryValueCondition<TField, TCompareValue> : ReactiveObject, IQueryValueCondition, IMementoProvider<QueryConditionValueMemento>
    where TField : notnull {
    public virtual int Priority => 0;
    public virtual Type FieldTypeClass => typeof(TField);
    public Type ActualFieldType { get; set; } = typeof(TField);
    public virtual Type CompareValueType => typeof(TCompareValue);

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

    protected QueryValueCondition(IList<CompareFunction<TField, TCompareValue>> functions) {
        _functions = functions;
        SelectedFunction = _selectedFunction = _functions.First();

        Summary = this.WhenAnyValue(
            x => x.SelectedFunction,
            x => x.CompareValue,
            (function, value) => function.Operator + " " + value);
    }

    public override string ToString() {
        return SelectedFunction.Operator + " " + CompareValue;
    }

    public virtual bool Accepts(Type type) => type.InheritsFrom(FieldTypeClass);
    public bool Evaluate(object? fieldValue) {
        if (CompareValue is null || fieldValue is not TField field) return false;

        return SelectedFunction.Evaluate(field, CompareValue);
    }

    public QueryConditionValueMemento CreateMemento() {
        return new QueryConditionValueMemento(
            ActualFieldType.AssemblyQualifiedName ?? string.Empty,
            SelectedFunction.Operator,
            CompareValue);
    }
    public void RestoreMemento(QueryConditionValueMemento memento) {
        ActualFieldType = Type.GetType(memento.FullTypeName) ?? typeof(TField);
        SelectedFunction = _functions.FirstOrDefault(x => x.Operator == memento.SelectedFunctionOperator) ?? _functions.First();
        CompareValue = memento.CompareValue;
    }

    IQueryConditionMemento IMementoProvider<IQueryConditionMemento>.CreateMemento() => CreateMemento();
    public void RestoreMemento(IQueryConditionMemento memento) {
        if (memento is QueryConditionValueMemento valueDto) {
            RestoreMemento(valueDto);
        }
    }
}
