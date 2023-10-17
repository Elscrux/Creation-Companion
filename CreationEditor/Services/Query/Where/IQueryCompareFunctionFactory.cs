namespace CreationEditor.Services.Query.Where;

public interface IQueryCompareFunctionFactory {
    IEnumerable<ICompareFunction> Get(Type type);
}
