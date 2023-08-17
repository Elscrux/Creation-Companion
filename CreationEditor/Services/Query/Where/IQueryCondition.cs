using CreationEditor.Core;
namespace CreationEditor.Services.Query.Where;

public interface IQueryConditionMemento {
    string SelectedFunctionOperator { get; init; }
}

public interface IQueryCondition : IMementoProvider<IQueryConditionMemento> {
    int Priority { get; }

    Type FieldTypeClass { get; }
    Type CompareValueType { get; }
    Type ActualFieldType { get; set; }
    IList<ICompareFunction> Functions { get; }
    ICompareFunction SelectedFunction { get; set; }

    IObservable<string> Summary { get; }

    bool Accepts(Type type);
    bool Evaluate(object? fieldValue);
}
