using System.Reactive;
using System.Reactive.Linq;
using Autofac;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Services.Query;

public interface IQueryRunner {
    IQueryFrom QueryFrom { get; }
    IObservableCollection<IQueryConditionEntry> QueryConditions { get; }
    IRecordFieldSelector RecordFieldSelector { get; }
    public IRecordFieldSelector OrderBySelector { get; }

    IObservable<Unit> SettingsChanged { get; }
    IObservable<string> Summary { get; }

    IEnumerable<object?> RunQuery();
}

public sealed class QueryRunner : IQueryRunner, IDisposable {
    private readonly IDisposableDropoff _disposables = new DisposableBucket();

    public ILifetimeScope LifetimeScope { get; }
    public IQueryFrom QueryFrom { get; }
    public IObservableCollection<IQueryConditionEntry> QueryConditions { get; } = new ObservableCollectionExtended<IQueryConditionEntry>();
    public IRecordFieldSelector RecordFieldSelector { get; }
    public IRecordFieldSelector OrderBySelector { get; }

    public IObservable<Unit> SettingsChanged { get; }
    public IObservable<string> Summary { get; }

    public QueryRunner(
        ILifetimeScope lifetimeScope,
        IQueryFrom queryFrom,
        IRecordFieldSelector recordFieldSelector,
        IRecordFieldSelector orderBySelector) {
        LifetimeScope = lifetimeScope;
        QueryFrom = queryFrom;
        RecordFieldSelector = recordFieldSelector;
        OrderBySelector = orderBySelector;

        var conditionChanges = QueryConditions
            .ObserveCollectionChanges()
            .Select(_ => QueryConditions.Select(x => x.ConditionEntryChanged).Merge())
            .Switch();

        var selectionChanged = this.WhenAnyValue(
            x => x.QueryFrom.SelectedItem,
            x => x.OrderBySelector.SelectedField,
            x => x.RecordFieldSelector.SelectedField,
            (from, orderBy, select) => (From: from, OrderBy: orderBy, Select: select));

        SettingsChanged = conditionChanges.Merge(selectionChanged.Unit());

        Summary = conditionChanges
            .Select(_ => QueryConditions.Select(c => c.Summary).CombineLatest())
            .Switch()
            .StartWith(Array.Empty<string>())
            .CombineLatest(selectionChanged, ((list, x) => list.Any() ? $"""
                    From {x.From?.Name ?? "None"}
                    Where
                    {string.Join('\n', list)}
                    Order By {x.OrderBy?.Name ?? "None"}
                    Select {x.Select?.Name ?? "None"}
                    """ : $"""
                    From {x.From?.Name ?? "None"}
                    Order By {x.OrderBy?.Name ?? "None"}
                    Select {x.Select?.Name ?? "None"}
                    """));

        this.WhenAnyValue(x => x.QueryFrom.SelectedItem)
            .NotNull()
            .Subscribe(item => {
                RecordFieldSelector.RecordType = item.Type;
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
        return RecordFieldSelector.SelectedField is null
            ? records
            : records.Select(record => RecordFieldSelector.SelectedField.GetValue(record));
    }

    public void Dispose() => _disposables.Dispose();
}
