namespace CreationEditor.Services.Query.From;

public sealed class QueryFromFactory : IQueryFromFactory {
    private readonly Func<QueryFromRecordType> _queryFromRecordTypeFactory;
    public QueryFromFactory(
        Func<QueryFromRecordType> queryFromRecordTypeFactory) {
        _queryFromRecordTypeFactory = queryFromRecordTypeFactory;
    }
    public IQueryFrom CreateFromRecordType(Type? type = null) {
        var queryFromRecordType = _queryFromRecordTypeFactory();
        if (type is not null) {
            queryFromRecordType.SelectedItem = queryFromRecordType.Items.FirstOrDefault(i => i.Type == type);
        }

        return queryFromRecordType;
    }
}
