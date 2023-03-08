using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
namespace CreationEditor;

public class ObservableCollectionSelectorAdaptor<T, TTarget> {
    private readonly Func<T, TTarget> _selector;
    private readonly IObservableCollection<TTarget> _collection;

    private readonly int _refreshThreshold;

    private bool _loaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollectionAdaptor{T}"/> class.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="collection">The collection.</param>
    /// <param name="refreshThreshold">The refresh threshold.</param>
    /// <exception cref="System.ArgumentNullException">collection.</exception>
    public ObservableCollectionSelectorAdaptor(Func<T, TTarget> selector, IObservableCollection<TTarget> collection, int refreshThreshold = 25) {
        _selector = selector;
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        _refreshThreshold = refreshThreshold;
    }

    /// <summary>
    /// Maintains the specified collection from the changes.
    /// </summary>
    /// <param name="changes">The changes.</param>
    public IChangeSet<TTarget> Adapt(IChangeSet<T> changes) {
        if (changes is null) throw new ArgumentNullException(nameof(changes));

        var newChanges = new ChangeSet<TTarget>();
        foreach (var change in changes) {
            switch (change.Type) {
                case ChangeType.Item:
                    var previous = change.Item.Previous.HasValue
                        ? Optional<TTarget>.Create(_selector(change.Item.Previous.Value))
                        : Optional<TTarget>.None;

                    var current = _selector(change.Item.Current);

                    newChanges.Add(new Change<TTarget>(change.Reason, current, previous, change.Item.CurrentIndex, change.Item.PreviousIndex));
                    break;
                case ChangeType.Range:
                    var transformed = change.Range.Select(_selector);
                    newChanges.Add(new Change<TTarget>(change.Reason, transformed, change.Range.Index));

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (newChanges.TotalChanges - newChanges.Refreshes > _refreshThreshold || !_loaded) {
            using (_collection.SuspendNotifications()) {
                _collection.Clone(newChanges);
                _loaded = true;
            }
        } else {
            _collection.Clone(newChanges);
        }

        return newChanges;
    }
}
