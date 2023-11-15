namespace CreationEditor.Services.Query.From;

public sealed class QueryFromFactory(Func<QueryFromRecordType> queryFromRecordTypeFactory) : IQueryFromFactory {
    public IQueryFrom CreateFromRecordType(Type? type = null) {
        var queryFromRecordType = queryFromRecordTypeFactory();
        if (type is not null) {
            queryFromRecordType.SelectedItem = queryFromRecordType.Items.FirstOrDefault(i => i.Type == type);
        }

        return queryFromRecordType;
    }
}
