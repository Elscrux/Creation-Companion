using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using CreationEditor.Services.Environment;
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
    public IQueryFieldSelector OrderBySelector { get; } = new ReflectionQueryFieldSelector();
    public IQueryFieldSelector FieldSelector { get; } = new ReflectionQueryFieldSelector();

    public IObservable<Unit> SettingsChanged { get; }
    public IObservable<string> Summary { get; }

    public bool EnableConditions { get; set; } = true;
    public bool EnableOrderBy { get; set; } = true;
    public bool EnableSelect { get; set; } = true;

    public QueryRunner(
        ILinkCacheProvider linkCacheProvider,
        IQueryFromFactory queryFromFactory,
        IQueryConditionFactory queryConditionFactory) {
        _queryConditionFactory = queryConditionFactory;

        QueryFrom = queryFromFactory.CreateFromRecordType();

        var observeCollectionChanges = QueryConditions.ObserveCollectionChanges();
        var conditionChanges = observeCollectionChanges
            .Select(_ => QueryConditions
                .Select(x => x.ConditionChanged)
                .Merge())
            .Switch()
            .Merge(observeCollectionChanges.Unit());

        var selectionChanged = this.WhenAnyValue(
            x => x.QueryFrom.SelectedItem,
            x => x.OrderBySelector.SelectedField,
            x => x.FieldSelector.SelectedField,
            (from, orderBy, select) => (From: from, OrderBy: orderBy, Select: select));

        SettingsChanged = conditionChanges
            .Merge(selectionChanged.Unit())
            .Merge(linkCacheProvider.LinkCacheChanged.Unit());

        Summary = conditionChanges
            .Select(_ => QueryConditions
                .Select(c => c.Summary)
                .CombineLatest()
                .StartWith(Array.Empty<string>()))
            .Switch()
            .CombineLatest(selectionChanged, (conditions, query) => (Conditions: conditions, Query: query))
            .ThrottleMedium()
            .Select(CreateSummary);

        this.WhenAnyValue(x => x.QueryFrom.SelectedItem)
            .NotNull()
            .Subscribe(item => {
                FieldSelector.RecordType = item.Type;
                OrderBySelector.RecordType = item.Type;
                QueryConditions.Clear();
            })
            .DisposeWith(_disposables);
    }

    private string CreateSummary((IList<string> Conditions, (QueryFromItem? From, IQueryField? OrderBy, IQueryField? Select) Query) x) {
        var builder = new StringBuilder();

        builder.AppendLine($"From {x.Query.From?.Name ?? "None"}");

        if (EnableConditions && x.Conditions.Any()) {
            builder.AppendLine("Where");
            foreach (var condition in x.Conditions) {
                builder.AppendLine(condition);
            }
        }

        if (EnableOrderBy) {
            builder.AppendLine($"Order By {x.Query.OrderBy?.Name ?? "None"}");
        }

        if (EnableSelect) {
            builder.AppendLine($"Select {x.Query.Select?.Name ?? "None"}");
        }

        return builder.ToString();
    }

    public IEnumerable<object?> RunQuery() {
        // From
        var records = QueryFrom.GetRecords();

        // Where
        if (EnableConditions && QueryConditions.Any()) {
            records = records.Where(record => QueryConditions.EvaluateConditions(record));
        }

        // Order By
        if (EnableOrderBy && OrderBySelector.SelectedField is not null) {
            records = records.OrderBy(record => OrderBySelector.SelectedField.GetValue(record));
        }

        // Select
        return EnableSelect && FieldSelector.SelectedField is not null
            ? records.Select(record => FieldSelector.SelectedField.GetValue(record))
            : records;
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
        QueryConditions.ReplaceWith(memento.QueryConditions.Select(entryMemento => {
            var queryCondition = _queryConditionFactory.Create();
            queryCondition.RestoreMemento(entryMemento);
            return queryCondition;
        }));
        OrderBySelector.RestoreMemento(memento.OrderBySelector);
        FieldSelector.RestoreMemento(memento.FieldSelector);
    }

    public void Dispose() => _disposables.Dispose();
}
