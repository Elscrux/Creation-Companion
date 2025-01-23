using CreationEditor.Core;
using ReactiveUI;
namespace CreationEditor.Services.Query.Select;

public sealed record QueryFieldSelectorMemento(QueryFieldMemento? SelectedField, string RecordTypeName);

public interface IQueryFieldSelector : IReactiveObject, IMementoProvider<QueryFieldSelectorMemento> {
    IQueryField? SelectedField { get; set; }

    Type? RecordType { get; set; }
}
