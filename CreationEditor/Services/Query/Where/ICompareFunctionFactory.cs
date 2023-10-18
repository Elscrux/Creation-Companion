namespace CreationEditor.Services.Query.Where;

public interface ICompareFunctionFactory {
    IEnumerable<ICompareFunction> Get(Type type);
}
