namespace CreationEditor.Services.Query.Where;

public interface IQueryConditionEntryFactory {
    IQueryConditionEntry Create(Type? type = null);
}
