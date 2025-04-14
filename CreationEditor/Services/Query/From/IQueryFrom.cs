using CreationEditor.Core;
using CreationEditor.Services.Query.Select;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Query.From;

public sealed record QueryFromMemento(QueryFieldMemento? SelectedItem);

public interface IQueryFrom : IMementoProvider<QueryFromMemento>, IMementoReceiver<QueryFromMemento> {
    QueryFromItem? SelectedItem { get; set; }
    IList<QueryFromItem> Items { get; }

    IEnumerable<IMajorRecordGetter> GetRecords();
}
