namespace CreationEditor.Services.Query.Where;

public sealed record CompareFunction<TField, TValue>(string Operator, Func<TField, TValue, bool> Evaluate) : ICompareFunction {
    bool ICompareFunction.Evaluate(object field, object value) {
        if (field is TField fieldT && value is TValue valueT) {
            return Evaluate(fieldT, valueT);
        }
        return false;
    }
}
