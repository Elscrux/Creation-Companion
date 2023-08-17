namespace CreationEditor.Services.Query.Where;

public interface ICompareFunction {
    string Operator { get; }

    bool Evaluate(object field, object value);
}
