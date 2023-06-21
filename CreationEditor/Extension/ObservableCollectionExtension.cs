using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using DynamicData;
using DynamicData.Binding;
using Noggog;
namespace CreationEditor;

public static class ObservableCollectionExtension {
    public static ReadOnlyObservableCollection<TTarget> SelectObservableCollection<TSource, TTarget>(
        this IObservableCollection<TSource> source,
        Expression<Func<TSource, TTarget>> selector,
        IDisposableDropoff disposable)
        where TSource : INotifyPropertyChanged {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        return source
            .ToObservableChangeSet<IObservableCollection<TSource>, TSource>()
            .AutoRefresh(selector)
            .ToObservableCollection(selector.Compile(), disposable);
    }

    public static ReadOnlyObservableCollection<TTarget> SelectObservableCollectionSync<TSource, TTarget>(
        this IObservableCollection<TSource> source,
        Func<TSource, TTarget> selector,
        IDisposableDropoff disposable) {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        var internalCollection = new ObservableCollectionExtended<TTarget>(source.Select(selector));

        source.ObserveCollectionChanges()
            .Subscribe(e => {
                internalCollection.Apply(e.EventArgs.Transform(selector));
            })
            .DisposeWith(disposable);

        return new ReadOnlyObservableCollection<TTarget>(internalCollection);
    }

    public static ReadOnlyObservableCollection<T> Combine<T>(this IObservableCollection<T> lhs, IDisposableDropoff disposableDropoff, params IObservableCollection<T>[] rhsList) {
        var internalCollection = new ObservableCollectionExtended<T>();
        foreach (var rhs in rhsList) {
            internalCollection.AddRange(rhs);
        }

        lhs.ObserveCollectionChanges()
            .Subscribe(e => {
                if (e.EventArgs.Action == NotifyCollectionChangedAction.Reset) {
                    internalCollection.Clear();

                    foreach (var rhs in rhsList) {
                        internalCollection.AddRange(rhs);
                    }
                } else {
                    internalCollection.Apply(e.EventArgs);
                }
            })
            .DisposeWith(disposableDropoff);

        rhsList
            .Select(x => x.ObserveCollectionChanges())
            .Merge()
            .Subscribe(e => {
                if (e.EventArgs.Action == NotifyCollectionChangedAction.Reset) {
                    internalCollection.Clear();

                    internalCollection.AddRange(lhs);
                    foreach (var rhs in rhsList) {
                        internalCollection.AddRange(rhs);
                    }
                } else {
                    internalCollection.Apply(e.EventArgs);
                }
            })
            .DisposeWith(disposableDropoff);

        return new ReadOnlyObservableCollection<T>(internalCollection);
    }

    public static void Apply<T>(this ObservableCollection<T> source, NotifyCollectionChangedEventArgs change, IEqualityComparer<T>? equalityComparer = null) {
        var comparer = equalityComparer ?? EqualityComparer<T>.Default;

        switch (change.Action) {
            case NotifyCollectionChangedAction.Add:
                if (change.NewItems is null) break;

                source.AddOrInsertRange(change.NewItems.OfType<T>(), change.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Remove:
                if (change.OldItems is null) break;
                var removeItems = change.OldItems.OfType<T>();

                source.RemoveMany(removeItems);
                break;
            case NotifyCollectionChangedAction.Replace:
                if (change.OldItems is null || change.NewItems is null) break;

                source.RemoveMany(change.OldItems.OfType<T>());
                source.AddOrInsertRange(change.NewItems.OfType<T>(), change.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Move:
                if (change.NewItems is null) break;

                var moveItems = change.NewItems.OfType<T>().ToArray();
                if (moveItems.Length == 0) break;

                var oldIndex = change.OldStartingIndex == -1 ? source.IndexOf(moveItems[0], comparer) : change.OldStartingIndex;
                var newIndex = change.NewStartingIndex;
                if (oldIndex == newIndex) break;

                source.RemoveRange(oldIndex, moveItems.Length);

                if (oldIndex < newIndex) {
                    source.AddOrInsertRange(moveItems, newIndex);
                } else {
                    source.AddOrInsertRange(moveItems, newIndex - moveItems.Length);
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                source.Clear();
                if (change.NewItems is null) break;

                source.AddRange(change.NewItems.OfType<T>());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void Apply<T>(this IList<T> source, Change<T> item, IEqualityComparer<T>? equalityComparer = null) {
        source.Clone(item.AsEnumerable(), equalityComparer);
    }

    public static void RemoveRange<T>(this IObservableCollection<T> collection, IEnumerable<T> itemsToRemove) {
        ArgumentNullException.ThrowIfNull(itemsToRemove);

        var itemsField = typeof(Collection<T>).GetField("items", BindingFlags.NonPublic | BindingFlags.Instance)!;
        if (itemsField.GetValue(collection) is not IList<T> internalList) throw new InvalidOperationException("Unable to get internal list");

        var list = itemsToRemove.ToList();
        int? smallestIndex = null;
        foreach (var item in list) {
            var index = internalList.IndexOf(item);
            if (index == -1) continue;

            if (index < smallestIndex || smallestIndex is null) {
                smallestIndex = index;
            }

            internalList.RemoveAt(index);
        }
        if (smallestIndex is null) return;

        var propertyChanged = typeof(ObservableCollection<T>).GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var collectionChanged = typeof(ObservableCollection<T>).GetMethod("OnCollectionChanged", BindingFlags.NonPublic | BindingFlags.Instance, new[] { typeof(NotifyCollectionChangedEventArgs) })!;
        propertyChanged.Invoke(collection, new object?[] { new PropertyChangedEventArgs(nameof(ObservableCollection<T>.Count)) });
        propertyChanged.Invoke(collection, new object?[] { new PropertyChangedEventArgs("Item[]") });
        collectionChanged.Invoke(collection, new object?[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list, smallestIndex.Value) });
    }

    private static void RemoveRange<T>(this IList<T> source, int index, int count) {
        ArgumentNullException.ThrowIfNull(source);

        switch (source) {
            case List<T> list:
                list.RemoveRange(index, count);
                break;
            case Noggog.IExtendedList<T> list:
                list.RemoveRange(index, count);
                break;
            default:
                throw new NotSupportedException($"Cannot remove range from {source.GetType().FullName}");
        }
    }

    public static void AddSorted<T>(this IList<T> list, T item) where T : IComparable {
        var i = 0;
        while (i < list.Count && list[i].CompareTo(item) < 0) i++;

        list.Insert(i, item);
    }

    public static void AddSorted<T>(this IList<T> list, T item, IComparer<T> comparer) {
        var i = 0;
        while (i < list.Count && comparer.Compare(list[i], item) < 0) i++;

        list.Insert(i, item);
    }

    public static void Sort<T, TKey>(this IObservableCollection<T> collection, Func<T, TKey> selector) => collection.ApplyOrder(collection.OrderBy(selector));

    public static void Sort<T>(this IObservableCollection<T> collection) where T : IComparable => collection.ApplyOrder(collection.Order());

    public static void ApplyOrder<T>(this IObservableCollection<T> collection, IOrderedEnumerable<T> order) {
        var sortedOrder = order.ToList();

        for (var i = 0; i < sortedOrder.Count; i++) {
            if (ReferenceEquals(collection[i], sortedOrder[i])) continue;

            collection.Move(collection.IndexOf(sortedOrder[i]), i);
        }
    }
}
