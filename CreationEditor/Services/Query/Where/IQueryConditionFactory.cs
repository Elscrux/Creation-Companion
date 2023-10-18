namespace CreationEditor.Services.Query.Where;

public interface IQueryConditionFactory {
    IQueryCondition Create(Type? type = null);
}
