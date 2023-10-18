using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Services.Query.From;
using CreationEditor.Services.Query.Select;
using CreationEditor.Services.Query.Where;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Services.Query;

public sealed class QueryRunner : IQueryRunner, IDisposable {
    private readonly IDisposableDropoff _disposables = new DisposableBucket();
    private readonly IQueryConditionFactory _queryConditionFactory;

    public Guid Id { get; } = Guid.NewGuid();

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

        var conditionCountChanges = QueryConditions
            .ObserveCollectionChanges()
            .Unit();

        var conditionEntryChanges = QueryConditions
            .ObserveCollectionChanges()
            .Select(_ => QueryConditions.Select(x => x.ConditionEntryChanged).Merge())
            .Switch();

        var conditionChanges = conditionEntryChanges.Merge(conditionCountChanges);

        var selectionChanged = this.WhenAnyValue(
            x => x.QueryFrom.SelectedItem,
            x => x.OrderBySelector.SelectedField,
            x => x.FieldSelector.SelectedField,
            (from, orderBy, select) => (From: from, OrderBy: orderBy, Select: select));

        SettingsChanged = conditionChanges.Merge(selectionChanged.Unit());

        Summary = conditionChanges
            .Select(_ => QueryConditions.Select(c => c.Summary).CombineLatest())
            .Switch()
            .StartWith(Array.Empty<string>())
            .CombineLatest(selectionChanged, (list, x) => list.Any()
                ? $"""
                   From {x.From?.Name ?? "None"}
                   Where
                   {string.Join('\n', list)}
                   Order By {x.OrderBy?.Name ?? "None"}
                   Select {x.Select?.Name ?? "None"}
                   """
                : $"""
                   From {x.From?.Name ?? "None"}
                   Order By {x.OrderBy?.Name ?? "None"}
                   Select {x.Select?.Name ?? "None"}
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
            QueryFrom.CreateMemento(),
            QueryConditions.Select(x => x.CreateMemento()).ToList(),
            OrderBySelector.CreateMemento(),
            FieldSelector.CreateMemento());
    }

    public void RestoreMemento(QueryRunnerMemento memento) {
        QueryFrom.RestoreMemento(memento.QueryFrom);
        QueryConditions.Clear();
        QueryConditions.AddRange(memento.QueryConditions.Select(entryMemento => {
            var queryConditionEntry = _queryConditionFactory.Create();
            queryConditionEntry.RestoreMemento(entryMemento);
            return queryConditionEntry;
        }));
        OrderBySelector.RestoreMemento(memento.OrderBySelector);
        FieldSelector.RestoreMemento(memento.FieldSelector);
    }

    public void Dispose() => _disposables.Dispose();
}
