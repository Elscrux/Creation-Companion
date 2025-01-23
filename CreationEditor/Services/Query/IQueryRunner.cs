using System.Reactive;
using CreationEditor.Core;
using CreationEditor.Services.Query.From;
using CreationEditor.Services.Query.Select;
using CreationEditor.Services.Query.Where;
using DynamicData.Binding;
namespace CreationEditor.Services.Query;

public sealed record QueryRunnerMemento(
    Guid Id,
    string Name,
    QueryFromMemento QueryFrom,
    List<QueryConditionMemento> QueryConditions,
    QueryFieldSelectorCollectionMemento OrderBySelector,
    QueryFieldSelectorCollectionMemento FieldSelector);

public interface IQueryRunner : IMementoProvider<QueryRunnerMemento> {
    Guid Id { get; }
    string Name { get; set; }
    IQueryFrom QueryFrom { get; }
    IObservableCollection<IQueryCondition> QueryConditions { get; }
    IQueryFieldSelectorCollection FieldSelector { get; }
    IQueryFieldSelectorCollection OrderBySelector { get; }

    IObservable<Unit> SettingsChanged { get; }
    IObservable<string> Summary { get; }

    bool EnableConditions { get; set; }
    bool EnableOrderBy { get; set; }
    bool EnableSelect { get; set; }

    IEnumerable<object?> RunQuery();
}
