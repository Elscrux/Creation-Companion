using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using DynamicData;
using DynamicData.Binding;
using Noggog;
namespace CreationEditor;

public static class ObservableCollectionExtension {
    public static ReadOnlyObservableCollection<TTarget> SelectObservableCollection<TSource, TTarget>(
        this ObservableCollection<TSource> source,
        Expression<Func<TSource, TTarget>> selector,
        IDisposableDropoff disposable)
        where TSource : INotifyPropertyChanged {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (selector is null) throw new ArgumentNullException(nameof(selector));

        return source
            .ToObservableChangeSet()
            .AutoRefresh(selector)
            .ToObservableCollection(selector.Compile(), disposable);
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
                if (item.Range.Index >= 0 && source is DynamicData.IExtendedList<T> or List<T>) {
                    source.RemoveRange(item.Range.Index, item.Range.Count);
                } else {
                    foreach (var remove in item.Range) {
                        var index = source.IndexOf(remove, comparer);
                        if (index > -1) {
                            source.RemoveAt(index);
                        }
                    }
                }

                break;
            }
            case ListChangeReason.Moved: {
                var change = item.Item;
                if (change.CurrentIndex < 0) {
                    throw new UnspecifiedIndexException("Cannot move as an index was not specified");
                }

                switch (source) {
                    case DynamicData.IExtendedList<T> extendedList:
                        extendedList.Move(change.PreviousIndex, change.CurrentIndex);
                        break;
                    case ObservableCollection<T> observableCollection:
                        observableCollection.Move(change.PreviousIndex, change.CurrentIndex);
                        break;
                    default:
                        // check this works whatever the index is
                        source.RemoveAt(change.PreviousIndex);
                        source.Insert(change.CurrentIndex, change.Current);
                        break;
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> itemsToAdd) {
        if (itemsToAdd == null) {
            throw new ArgumentNullException(nameof(itemsToAdd));
        }

        var itemsField = typeof(Collection<T>).GetField("items", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var internalList = itemsField.GetValue(collection) as IList<T>;
        if (internalList == null) throw new InvalidOperationException("Unable to get internal list");

        var list = itemsToAdd.ToList();
        foreach (var item in list) {
            internalList.Add(item);
        }
        var propertyChanged = typeof(ObservableCollection<T>).GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var collectionChanged = typeof(ObservableCollection<T>).GetMethod("OnCollectionChanged", BindingFlags.NonPublic | BindingFlags.Instance, types: new[] { typeof(NotifyCollectionChangedEventArgs) })!;
        propertyChanged.Invoke(collection, new object?[] { new PropertyChangedEventArgs(nameof(ObservableCollection<T>.Count)) });
        propertyChanged.Invoke(collection, new object?[] { new PropertyChangedEventArgs("Item[]") });
        collectionChanged.Invoke(collection, new object?[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, 0) });
    }

    public static void RemoveRange<T>(this ObservableCollection<T> collection, IEnumerable<T> itemsToRemove) {
        if (itemsToRemove == null) {
            throw new ArgumentNullException(nameof(itemsToRemove));
        }

        var itemsField = typeof(Collection<T>).GetField("items", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var internalList = itemsField.GetValue(collection) as IList<T>;
        if (internalList == null) throw new InvalidOperationException("Unable to get internal list");

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
        var collectionChanged = typeof(ObservableCollection<T>).GetMethod("OnCollectionChanged", BindingFlags.NonPublic | BindingFlags.Instance, types: new[] { typeof(NotifyCollectionChangedEventArgs) })!;
        propertyChanged.Invoke(collection, new object?[] { new PropertyChangedEventArgs(nameof(ObservableCollection<T>.Count)) });
        propertyChanged.Invoke(collection, new object?[] { new PropertyChangedEventArgs("Item[]") });
        collectionChanged.Invoke(collection, new object?[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list, smallestIndex.Value) });
    }

    private static void RemoveRange<T>(this IList<T> source, int index, int count) {
        if (source is null) {
            throw new ArgumentNullException(nameof(source));
        }

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
