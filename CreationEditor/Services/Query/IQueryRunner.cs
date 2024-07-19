using System.Reactive;
using CreationEditor.Core;
using CreationEditor.Services.Query.From;
using CreationEditor.Services.Query.Select;
using CreationEditor.Services.Query.Where;
using DynamicData.Binding;
using ReactiveUI;
namespace CreationEditor.Services.Query;

public sealed record QueryRunnerMemento(
    Guid Id,
    string Name,
    QueryFromMemento QueryFrom,
    List<QueryConditionMemento> QueryConditions,
    QueryFieldSelectorMemento OrderBySelector,
    QueryFieldSelectorMemento FieldSelector);

public interface IQueryRunner : IReactiveObject, IMementoProvider<QueryRunnerMemento> {
    Guid Id { get; }
    string Name { get; set; }
    IQueryFrom QueryFrom { get; }
    IObservableCollection<IQueryCondition> QueryConditions { get; }
    IQueryFieldSelector FieldSelector { get; }
    public IQueryFieldSelector OrderBySelector { get; }

    IObservable<Unit> SettingsChanged { get; }
    IObservable<string> Summary { get; }

    bool EnableConditions { get; set; }
    bool EnableOrderBy { get; set; }
    bool EnableSelect { get; set; }

    IEnumerable<object?> RunQuery();
}
