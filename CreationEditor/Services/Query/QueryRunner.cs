using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Services.Query.From;
using CreationEditor.Services.Query.Select;
using CreationEditor.Services.Query.Where;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query;

public sealed class QueryRunner : ReactiveObject, IQueryRunner, IDisposable {
    private readonly DisposableBucket _disposables = new();
    private readonly IQueryConditionFactory _queryConditionFactory;

    public Guid Id { get; private set; } = Guid.NewGuid();
    [Reactive] public string Name { get; set; } = string.Empty;

    public IQueryFrom QueryFrom { get; }
    public IObservableCollection<IQueryCondition> QueryConditions { get; } = new ObservableCollectionExtended<IQueryCondition>();
    public IFieldSelector OrderBySelector { get; } = new ReflectionFieldSelector();
    public IFieldSelector FieldSelector { get; } = new ReflectionFieldSelector();

    public IObservable<Unit> SettingsChanged { get; }
    public IObservable<string> Summary { get; }

    public QueryRunner(
        IQueryFromFactory queryFromFactory,
        IQueryConditionFactory queryConditionFactory) {
        _queryConditionFactory = queryConditionFactory;

        QueryFrom = queryFromFactory.CreateFromRecordType();

        var observeCollectionChanges = QueryConditions.ObserveCollectionChanges();
        var conditionChanges = observeCollectionChanges
            .Select(_ => {
                return QueryConditions
                    .Select(x => x.ConditionChanged)
                    .Merge();
            })
            .Switch()
            .Merge(observeCollectionChanges
                .Unit());

        var selectionChanged = this.WhenAnyValue(
            x => x.QueryFrom.SelectedItem,
            x => x.OrderBySelector.SelectedField,
            x => x.FieldSelector.SelectedField,
            (from, orderBy, select) => (From: from, OrderBy: orderBy, Select: select));

        SettingsChanged = conditionChanges.Merge(selectionChanged.Unit());

        Summary = conditionChanges
            .Select(_ => QueryConditions
                .Select(c => c.Summary)
                .CombineLatest()
                .StartWith([]))
            .Switch()
            .CombineLatest(selectionChanged, (conditions, query) => (Conditions: conditions, Query: query))
            .ThrottleMedium()
            .Select(x => x.Conditions.Any()
                ? $"""
                   From {x.Query.From?.Name ?? "None"}
                   Where
                   {string.Join('\n', x.Conditions)}
                   Order By {x.Query.OrderBy?.Name ?? "None"}
                   Select {x.Query.Select?.Name ?? "None"}
                   """
                : $"""
                   From {x.Query.From?.Name ?? "None"}
                   Order By {x.Query.OrderBy?.Name ?? "None"}
                   Select {x.Query.Select?.Name ?? "None"}
                   """);

        this.WhenAnyValue(x => x.QueryFrom.SelectedItem)
            .NotNull()
            .Subscribe(item => {
                FieldSelector.RecordType = item.Type;
                OrderBySelector.RecordType = item.Type;
                QueryConditions.Clear();
            })
            .DisposeWith(_disposables);
    }

    public IEnumerable<object?> RunQuery() {
        // From
        var records = QueryFrom.GetRecords();

        // Where
        if (QueryConditions.Any()) {
            records = records.Where(record => QueryConditions.EvaluateConditions(record));
        }

        // Order By
        if (OrderBySelector.SelectedField is not null) {
            records = records.OrderBy(record => OrderBySelector.SelectedField.GetValue(record));
        }

        // Select
        return FieldSelector.SelectedField is null
            ? records
            : records.Select(record => FieldSelector.SelectedField.GetValue(record));
    }

    public QueryRunnerMemento CreateMemento() {
        return new QueryRunnerMemento(
            Id,
            Name,
            QueryFrom.CreateMemento(),
            QueryConditions.Select(x => x.CreateMemento()).ToList(),
            OrderBySelector.CreateMemento(),
            FieldSelector.CreateMemento());
    }

    public void RestoreMemento(QueryRunnerMemento memento) {
        Id = memento.Id;
        Name = memento.Name;
        QueryFrom.RestoreMemento(memento.QueryFrom);
        QueryConditions.Clear();
        QueryConditions.AddRange(memento.QueryConditions.Select(entryMemento => {
            var queryCondition = _queryConditionFactory.Create();
            queryCondition.RestoreMemento(entryMemento);
            return queryCondition;
        }));
        OrderBySelector.RestoreMemento(memento.OrderBySelector);
        FieldSelector.RestoreMemento(memento.FieldSelector);
    }

    public void Dispose() => _disposables.Dispose();
}
