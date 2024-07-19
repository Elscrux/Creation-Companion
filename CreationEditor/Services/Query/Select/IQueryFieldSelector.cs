using CreationEditor.Core;
using DynamicData.Binding;
namespace CreationEditor.Services.Query.Select;

public sealed record QueryFieldSelectorMemento(QueryFieldMemento? SelectedField, string RecordTypeName);

public interface IQueryFieldSelector : IMementoProvider<QueryFieldSelectorMemento> {
    IQueryField? SelectedField { get; set; }
    IObservableCollection<IQueryField> Fields { get; }

    Type? RecordType { get; set; }
}
