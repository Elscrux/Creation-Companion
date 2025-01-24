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
namespace CreationEditor.Services.Query;

public sealed class QueryRunner : IQueryRunner, IDisposable {
    private static readonly IReadOnlyList<Type> BlacklistedOrderTypes = [typeof(bool)];

    private readonly DisposableBucket _disposables = new();
    private readonly IQueryConditionFactory _queryConditionFactory;
    private readonly IQueryFieldProvider _queryFieldProvider = new ReflectionIQueryFieldProvider();

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public IQueryFrom QueryFrom { get; }
    public IObservableCollection<IQueryCondition> QueryConditions { get; } = new ObservableCollectionExtended<IQueryCondition>();
    public IQueryFieldSelectorCollection OrderBySelector { get; }
    public IQueryFieldSelectorCollection FieldSelector { get; } = new QueryFieldSelectorCollection();

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
        OrderBySelector = new QueryFieldSelectorCollection {
            SelectorFilter = field => {
                // Only allow fields whose types inherit from IComparable<T>
                // or have nested fields that inherit from IComparable<T>
                var remainingTypes = new Queue<Type>([field.Type]);
                var processedTypes = new HashSet<Type>();

                while (remainingTypes.Any()) {
                    var currentType = remainingTypes.Dequeue();
                    if (processedTypes.Contains(currentType)) continue;
                    if (BlacklistedOrderTypes.Contains(currentType)) continue;

                    if (currentType.InheritsFromOpenGeneric(typeof(IComparable<>))) {
                        return true;
                    }

                    processedTypes.Add(currentType);

                    var queryFields = _queryFieldProvider.FromType(currentType);
                    remainingTypes.Enqueue(queryFields.Select(x => x.Type));
                }

                return false;
            }
        };

        QueryFrom = queryFromFactory.CreateFromRecordType();

        var observeCollectionChanges = QueryConditions.ObserveCollectionChanges();
        var conditionChanges = observeCollectionChanges
            .Select(_ => QueryConditions
                .Select(x => x.ConditionChanged)
                .Merge())
            .Switch()
            .Merge(observeCollectionChanges.Unit());

        var selectionChanged = this.WhenAnyValue(x => x.QueryFrom.SelectedItem)
                .CombineLatest(
                    OrderBySelector.SelectionChanged,
                    FieldSelector.SelectionChanged,
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
            .CombineLatest(selectionChanged, (conditions, _) => conditions)
            .ThrottleMedium()
            .Select(CreateSummary);

        this.WhenAnyValue(x => x.QueryFrom.SelectedItem)
            .NotNull()
            .Subscribe(item => {
                FieldSelector.SetRootType(item.Type);
                OrderBySelector.SetRootType(item.Type);
                QueryConditions.Clear();
            })
            .DisposeWith(_disposables);
    }

    private string CreateSummary(IList<string> conditions) {
        var builder = new StringBuilder();

        builder.AppendLine($"From {QueryFrom.SelectedItem?.Name ?? "None"}");

        if (EnableConditions && conditions.Any()) {
            builder.AppendLine("Where");
            foreach (var condition in conditions) {
                builder.AppendLine(condition);
            }
        }

        if (EnableOrderBy) {
            builder.AppendLine($"Order By {OrderBySelector.GetFieldName()}");
        }

        if (EnableSelect) {
            builder.AppendLine($"Select {FieldSelector.GetFieldName()}");
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
        if (EnableOrderBy) {
            records = records.OrderBy(record => OrderBySelector.GetValue(record));
        }

        // Select
        return EnableSelect
            ? records.Select(record => FieldSelector.GetValue(record))
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
