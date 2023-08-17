namespace CreationEditor.Services.Query.From;

public interface IQueryFromFactory {
    IQueryFrom CreateFromRecordType(Type? type = null);
}
