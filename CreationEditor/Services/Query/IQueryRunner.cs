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
    FieldSelectorMemento OrderBySelector,
    FieldSelectorMemento FieldSelector);

public interface IQueryRunner : IReactiveObject, IMementoProvider<QueryRunnerMemento> {
    Guid Id { get; }
    string Name { get; set; }
    IQueryFrom QueryFrom { get; }
    IObservableCollection<IQueryCondition> QueryConditions { get; }
    IFieldSelector FieldSelector { get; }
    public IFieldSelector OrderBySelector { get; }

    IObservable<Unit> SettingsChanged { get; }
    IObservable<string> Summary { get; }

    IEnumerable<object?> RunQuery();
}
