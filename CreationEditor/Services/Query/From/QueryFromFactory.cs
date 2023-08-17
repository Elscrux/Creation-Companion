using Autofac;
namespace CreationEditor.Services.Query.From;

public sealed class QueryFromFactory : IQueryFromFactory {
    private readonly IComponentContext _componentContext;
    public QueryFromFactory(IComponentContext componentContext) {
        _componentContext = componentContext;
    }
    public IQueryFrom CreateFromRecordType(Type? type = null) {
        var queryFromRecordType = _componentContext.Resolve<QueryFromRecordType>();
        if (type is not null) {
            queryFromRecordType.SelectedItem = queryFromRecordType.Items.FirstOrDefault(i => i.Type == type);
        }

        return queryFromRecordType;
    }
}
