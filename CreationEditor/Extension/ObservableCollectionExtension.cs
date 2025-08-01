﻿using System.Collections;
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

public static class ObservableCollectionExtension{
    public static ReadOnlyObservableCollection<TTarget> SelectObservableCollection<TSource, TTarget>(
        this IObservableCollection<TSource> source,
        Expression<Func<TSource, TTarget>> selector,
        IDisposableDropoff disposable)
        where TSource : INotifyPropertyChanged
        where TTarget : notnull {
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
        IDisposableDropoff disposable)
        where TTarget : notnull {
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

    public static ReadOnlyObservableCollection<T> Combine<T>(
        this IObservableCollection<T> lhs,
        IDisposableDropoff disposableDropoff,
        params IReadOnlyList<IObservableCollection<T>> rhsList)
        where T : notnull {
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
                    internalCollection.LoadOptimized(lhs);
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

    public static void Apply<T>(
        this ObservableCollection<T> source,
        NotifyCollectionChangedEventArgs change,
        IEqualityComparer<T>? equalityComparer = null)
        where T : notnull {
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
                if (change.NewItems is null) {
                    source.Clear();
                } else {
                    source.ReplaceWith(change.NewItems.OfType<T>());
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(change));
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

            if (index < smallestIndex || smallestIndex is null) {
                smallestIndex = index;
            }

            internalList.RemoveAt(index);
        }
        if (smallestIndex is null) return;

        ObservableCollectionHelper<T>.NotifyChanges(collection);
    }

    public static void Sort<T, TKey>(this IObservableCollection<T> collection, Func<T, TKey> selector) =>
        collection.ApplyOrder(collection.OrderBy(selector));

    public static void Sort<T>(this IObservableCollection<T> collection)
        where T : IComparable => collection.ApplyOrder(collection.Order());

    public static void ApplyOrder<T>(this IObservableCollection<T> collection, IOrderedEnumerable<T> order) {
        var sortedOrder = order.ToList();

        for (var i = 0; i < sortedOrder.Count; i++) {
            if (ReferenceEquals(collection[i], sortedOrder[i])) continue;

            collection.Move(collection.IndexOf(sortedOrder[i]), i);
        }
    }

    public static void ApplyOrderNoMove<T>(this IObservableCollection<T> collection, IOrderedEnumerable<T> order) {
        var sortedOrder = order.ToList();

        for (var i = 0; i < sortedOrder.Count; i++) {
            var item = sortedOrder[i];
            if (ReferenceEquals(collection[i], item)) continue;

            collection.RemoveAt(collection.IndexOf(item));
            collection.Insert(i, item);
        }
    }

    public static void LoadOptimized<T>(this IObservableCollection<T> collection, IEnumerable<T> source) {
        var newItems = source.ToArray();

        if (ObservableCollectionHelper<T>.ItemsField.GetValue(collection) is IList items) {
            items.Clear();
            foreach (var newItem in newItems) {
                items.Add(newItem);
            }

            ObservableCollectionHelper<T>.NotifyChanges(collection);
        } else {
            collection.Load(newItems);
        }
    }

    private static class ObservableCollectionHelper<T> {
        public static readonly object[] CountPropertyChanged = [new PropertyChangedEventArgs(nameof(ObservableCollection<>.Count))];
        public static readonly object[] IndexerPropertyChanged = [new PropertyChangedEventArgs("Item[]")];
        public static readonly object[] ResetCollectionChanged = [new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)];

        public static readonly MethodInfo OnPropertyChanged = typeof(ObservableCollection<T>).GetMethod(
            "OnPropertyChanged",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        public static readonly MethodInfo OnCollectionChanged = typeof(ObservableCollection<T>).GetMethod(
            "OnCollectionChanged",
            BindingFlags.NonPublic | BindingFlags.Instance,
            [typeof(NotifyCollectionChangedEventArgs)])!;

        public static readonly FieldInfo ItemsField = typeof(Collection<T>).GetField(
            "items",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        public static void NotifyChanges(IObservableCollection<T> collection) {
            OnPropertyChanged.Invoke(collection, CountPropertyChanged);
            OnPropertyChanged.Invoke(collection, IndexerPropertyChanged);
            OnCollectionChanged.Invoke(collection, ResetCollectionChanged);
        }
    }
}
