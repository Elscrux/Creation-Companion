namespace CreationEditor.Services.Query.Where;

public interface IQueryCompareFunctionFactory {
    IEnumerable<IQueryCompareFunction> Get(Type type);
}
