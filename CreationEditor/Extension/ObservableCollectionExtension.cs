using System.Collections;
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
    extension<TSource>(IObservableCollection<TSource> source)
        where TSource : INotifyPropertyChanged {
        public ReadOnlyObservableCollection<TTarget> SelectObservableCollection<TTarget>(
            Expression<Func<TSource, TTarget>> selector,
            IDisposableDropoff disposable)
            where TTarget : notnull {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            return source
                .ToObservableChangeSet<IObservableCollection<TSource>, TSource>()
                .AutoRefresh(selector)
                .ToObservableCollection(selector.Compile(), disposable);
        }
    }

    extension<TSource>(IObservableCollection<TSource> source) where TSource : notnull {
        public ReadOnlyObservableCollection<TTarget> SelectObservableCollectionSync<TTarget>(
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
    }

    extension<T>(IObservableCollection<T> lhs)
        where T : notnull {
        public ReadOnlyObservableCollection<T> Combine(
            IDisposableDropoff disposableDropoff,
            params IReadOnlyList<IObservableCollection<T>> rhsList) {
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
    }

    extension<T>(ObservableCollection<T> source)
        where T : notnull {
        public void Apply(
            NotifyCollectionChangedEventArgs change,
            IEqualityComparer<T>? equalityComparer = null) {
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
    }

    extension<T>(IObservableCollection<T> collection)
        where T : IComparable {
        public void Sort() => collection.ApplyOrder(collection.Order());
    }

    extension<T>(IObservableCollection<T> collection) {
        public void RemoveRange(IEnumerable<T> itemsToRemove) {
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
        public void Sort<TKey>(Func<T, TKey> selector) =>
            collection.ApplyOrder(collection.OrderBy(selector));
        public void ApplyOrder(IOrderedEnumerable<T> order) {
            var sortedOrder = order.ToList();

            for (var i = 0; i < sortedOrder.Count; i++) {
                if (ReferenceEquals(collection[i], sortedOrder[i])) continue;

                collection.Move(collection.IndexOf(sortedOrder[i]), i);
            }
        }
        public void ApplyOrderNoMove(IOrderedEnumerable<T> order) {
            var sortedOrder = order.ToList();

            for (var i = 0; i < sortedOrder.Count; i++) {
                var item = sortedOrder[i];
                if (ReferenceEquals(collection[i], item)) continue;

                collection.RemoveAt(collection.IndexOf(item));
                collection.Insert(i, item);
            }
        }
        public void LoadOptimized(IEnumerable<T> source) {
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
