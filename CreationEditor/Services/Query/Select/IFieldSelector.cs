using CreationEditor.Core;
using DynamicData.Binding;
namespace CreationEditor.Services.Query.Select;

public sealed record FieldSelectorMemento(QueryFieldMemento? SelectedField, string RecordTypeName);

public interface IFieldSelector : IMementoProvider<FieldSelectorMemento> {
    IQueryField? SelectedField { get; set; }
    IObservableCollection<IQueryField> Fields { get; }

    Type? RecordType { get; set; }
}
