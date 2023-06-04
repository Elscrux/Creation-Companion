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

    public static ReadOnlyObservableCollection<T> Combine<T>(this IObservableCollection<T> lhs, IDisposableDropoff disposableDropoff, params IObservableCollection<T>[] rhsList) {
        var internalCollection = new ObservableCollectionExtended<T>();
        foreach (var rhs in rhsList) {
            internalCollection.AddRange(rhs);
        }

        Observable
            .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => lhs.CollectionChanged += h,
                h => lhs.CollectionChanged -= h)
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
            .Select(x => Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    h => x.CollectionChanged += h,
                    h => x.CollectionChanged -= h)).Merge()
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

    public static void Apply<T>(this IObservableCollection<T> source, NotifyCollectionChangedEventArgs change, IEqualityComparer<T>? equalityComparer = null) {
        var comparer = equalityComparer ?? EqualityComparer<T>.Default;

        switch (change.Action) {
            case NotifyCollectionChangedAction.Add:
                if (change.NewItems == null) break;

                source.AddOrInsertRange(change.NewItems.OfType<T>(), change.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Remove:
                if (change.OldItems == null) break;
                var removeItems = change.OldItems.OfType<T>();

                source.RemoveMany(removeItems);
                break;
            case NotifyCollectionChangedAction.Replace:
                if (change.OldItems == null || change.NewItems == null) break;

                source.RemoveMany(change.OldItems.OfType<T>());
                source.AddOrInsertRange(change.NewItems.OfType<T>(), change.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Move:
                if (change.NewItems == null) break;

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
                if (change.NewItems == null) break;

                source.AddRange(change.NewItems.OfType<T>());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void Apply<T>(this IList<T> source, Change<T> item, IEqualityComparer<T>? equalityComparer = null) {
        var comparer = equalityComparer ?? EqualityComparer<T>.Default;

        switch (item.Reason) {
            case ListChangeReason.Add: {
                var change = item.Item;
                var hasIndex = change.CurrentIndex >= 0;
                if (hasIndex) {
                    source.Insert(change.CurrentIndex, change.Current);
                } else {
                    source.Add(change.Current);
                }

                break;
            }
            case ListChangeReason.AddRange: {
                if (item.Range.Index.IsInRange(0, source.Count - 1)) {
                    source.AddOrInsertRange(item.Range, item.Range.Index);
                } else {
                    source.AddRange(item.Range, 0);
                }
                break;
            }
            case ListChangeReason.Clear: {
                source.Clear();
                break;
            }
            case ListChangeReason.Replace: {
                var change = item.Item;
                if (change.CurrentIndex >= 0 && change.CurrentIndex == change.PreviousIndex) {
                    source[change.CurrentIndex] = change.Current;
                } else {
                    if (change.PreviousIndex == -1) {
                        var index = source.IndexOf(change.Previous.Value, comparer);
                        if (index > -1) {
                            source.RemoveAt(index);
                        }
                    } else {
                        // is this best? or replace + move?
                        source.RemoveAt(change.PreviousIndex);
                    }

                    if (change.CurrentIndex == -1) {
                        source.Add(change.Current);
                    } else {
                        source.Insert(change.CurrentIndex, change.Current);
                    }
                }

                break;
            }
            case ListChangeReason.Refresh: {
                source.RemoveAt(item.Item.CurrentIndex);
                source.Insert(item.Item.CurrentIndex, item.Item.Current);

                break;
            }
            case ListChangeReason.Remove: {
                var change = item.Item;
                if (change.CurrentIndex >= 0) {
                    source.RemoveAt(change.CurrentIndex);
                } else {
                    var index = source.IndexOf(change.Current, comparer);
                    if (index > -1) {
                        source.RemoveAt(index);
                    }
                }

                break;
            }
            case ListChangeReason.RemoveRange: {
                // ignore this case because WhereReasonsAre removes the index [in which case call RemoveMany]
                //// if (item.Range.Index < 0)
                ////    throw new UnspecifiedIndexException("ListChangeReason.RemoveRange should not have an index specified index");
                if (item.Range.Index >= 0 && source is ObservableCollectionExtended<T> extended) {
                    extended.RemoveRange(item.Range.Index, item.Range.Count);
                } else {
                    foreach (var remove in item.Range) {
                        var index = source.IndexOf(remove, comparer);
                        if (index > -1) {
                            source.RemoveAt(index);
                        }
                    }
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

            if (index < smallestIndex || smallestIndex == null) {
                smallestIndex = index;
            }

            internalList.RemoveAt(index);
        }
        if (smallestIndex == null) return;

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
