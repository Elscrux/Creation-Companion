namespace CreationEditor.Services.Query;

public interface IQueryConditionProvider {
    IQueryCondition GetCondition(Type type);
}
