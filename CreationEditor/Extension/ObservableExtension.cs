using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor;

// Mostly copied from Noggog.CSharpExt
public static class ObservableExtension {
    public static IObservable<T> ObserveOnGui<T>(this IObservable<T> obs) => obs.ObserveOn(RxApp.MainThreadScheduler);
    public static IObservable<T> ObserveOnTaskpool<T>(this IObservable<T> obs) => obs.ObserveOn(RxApp.TaskpoolScheduler);

    public static IObservable<T> ThrottleShort<T>(this IObservable<T> obs) => obs.Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler);
    public static IObservable<T> ThrottleMedium<T>(this IObservable<T> obs) => obs.Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler);
    public static IObservable<T> ThrottleLong<T>(this IObservable<T> obs) => obs.Throttle(TimeSpan.FromMilliseconds(700), RxApp.MainThreadScheduler);
    public static IObservable<T> ThrottleVeryLong<T>(this IObservable<T> obs) =>
        obs.Throttle(TimeSpan.FromMilliseconds(1200), RxApp.MainThreadScheduler);

    public static IObservable<Unit> WhenCollectionChanges(this INotifyCollectionChanged source) =>
        source.ObserveCollectionChanges().Unit().StartWith(Unit.Default);

    public static IObservable<T> UpdateWhenCollectionChanges<T>(this IObservable<T> observable, INotifyCollectionChanged collection) {
        return observable.CombineLatest(collection.WhenCollectionChanges(), (t, _) => t);
    }

    public static IDisposable SyncTo<T>(
        this IObservable<IObservableCollection<T>> observableCollectionChanged,
        ObservableCollection<T> observableCollection)
        where T : notnull {
        return observableCollectionChanged
            .Subscribe(observableCollection.ReplaceWith);
    }

    public static IObservable<T> SelectWhenCollectionChanges<T>(this INotifyCollectionChanged source, Func<IObservable<T>> selector) {
        return source
            .WhenCollectionChanges()
            .Select(_ => selector())
            .Switch();
    }

    public static IObservable<T> DoOnGuiAndSwitchBack<T>(this IObservable<T> obs, Action<T> onNext) {
        return obs
            .ObserveOnGui()
            .Do(onNext)
            .ObserveOnTaskpool();
    }

    public static IObservable<T> DoOnGui<T>(this IObservable<T> obs, Action<T> onNext) {
        return obs
            .ObserveOnGui()
            .Do(onNext);
    }

    public static ReadOnlyObservableCollection<TObj> ToObservableCollectionSync<TObj, TKey>(
        this IObservable<ISortedChangeSet<TObj, TKey>> observable,
        IEnumerable<TObj> initial,
        IDisposableDropoff disposable)
        where TKey : notnull
        where TObj : notnull {
        var collection = new ObservableCollectionExtended<TObj>(initial);
        var adaptor = new SortedObservableCollectionAdaptor<TObj, TKey>();

        observable
            .Subscribe(sortedChangeSet => adaptor.Adapt(sortedChangeSet, collection))
            .DisposeWith(disposable);

        return new ReadOnlyObservableCollection<TObj>(collection);
    }

    public static ReadOnlyObservableCollection<TObj> ToObservableCollectionSync<TObj, TKey>(
        this IObservable<IChangeSet<TObj, TKey>> observable,
        IEnumerable<TObj> initial,
        IDisposableDropoff disposable)
        where TKey : notnull
        where TObj : notnull {
        var collection = new ObservableCollectionExtended<TObj>(initial);
        var adaptor = new ObservableCollectionAdaptor<TObj, TKey>();

        observable
            .Subscribe(changeSet => adaptor.Adapt(changeSet, collection))
            .DisposeWith(disposable);

        return new ReadOnlyObservableCollection<TObj>(collection);
    }

    public static IObservableCollection<TObj> ToObservableCollection<TObj, TKey>(
        this IObservable<IChangeSet<TObj, TKey>> changeSet,
        IDisposableDropoff disposable)
        where TKey : notnull
        where TObj : notnull {
        var destination = new ObservableCollectionExtended<TObj>();

        changeSet
            .ObserveOnGui()
            .Bind(destination)
            .Subscribe()
            .DisposeWith(disposable);

        return destination;
    }

    public static IObservableCollection<TObj> ToObservableCollection<TObj, TKey>(
        this IObservable<ISortedChangeSet<TObj, TKey>> changeSet,
        IDisposableDropoff disposable)
        where TKey : notnull
        where TObj : notnull {
        var destination = new ObservableCollectionExtended<TObj>();
        changeSet
            .ObserveOnGui()
            .Bind(destination)
            .Subscribe()
            .DisposeWith(disposable);

        return destination;
    }

    public static ReadOnlyObservableCollection<TTarget> ToObservableCollection<TObj, TKey, TTarget>(
        this IObservable<ISortedChangeSet<TObj, TKey>> changeSet,
        Func<TObj, TKey, TTarget> selector,
        IDisposableDropoff disposable)
        where TKey : notnull
        where TObj : notnull
        where TTarget : notnull {
        changeSet
            .ObserveOnGui()
            .Bind(selector, out var readOnlyObservableCollection)
            .Subscribe()
            .DisposeWith(disposable);

        return readOnlyObservableCollection;
    }

    public static ReadOnlyObservableCollection<TObj> ToObservableCollection<TObj>(
        this IObservable<IChangeSet<TObj>> changeSet,
        IDisposableDropoff disposable)
        where TObj : notnull {
        changeSet
            .ObserveOnGui()
            .Bind(out var readOnlyObservableCollection)
            .Subscribe()
            .DisposeWith(disposable);

        return readOnlyObservableCollection;
    }

    public static ReadOnlyObservableCollection<TTarget> ToObservableCollection<TObj, TTarget>(
        this IObservable<IChangeSet<TObj>> changeSet,
        Func<TObj, TTarget> selector,
        IDisposableDropoff disposable)
        where TObj : notnull
        where TTarget : notnull {
        changeSet
            .ObserveOnGui()
            .Bind(selector, out var readOnlyObservableCollection)
            .Subscribe()
            .DisposeWith(disposable);

        return readOnlyObservableCollection;
    }

    public static ReadOnlyObservableCollection<T> ToObservableCollection<T>(
        this IObservable<IEnumerable<T>> listObservable,
        IDisposableDropoff disposable)
        where T : notnull {
        return listObservable
            .ObserveOnGui()
            .Pairwise()
            .Select(x => {
                if (x.Current is null) return [];
                if (x.Previous is null) return new ChangeSet<T>(x.Current.Select(c => new Change<T>(ListChangeReason.Add, c)));

                var prev = x.Previous.ToArray();
                var cur = x.Current.ToArray();
                return new ChangeSet<T>(
                    prev
                        .Except(cur)
                        .Select(item => new Change<T>(ListChangeReason.Remove, item))
                        .Concat(
                            cur
                                .Except(prev)
                                .Select(item => new Change<T>(ListChangeReason.Add, item))));
            })
            .ToObservableCollection(disposable);
    }

    public static ReadOnlyObservableCollection<T> ToObservableCollection<T, TKey>(
        this IObservable<IEnumerable<T>> listObservable,
        Func<T, TKey> keySelector,
        IDisposableDropoff disposable)
        where T : notnull {
        return listObservable
            .ObserveOnGui()
            .Pairwise()
            .Select(x => {
                if (x.Current is null) return [];
                if (x.Previous is null) return new ChangeSet<T>(x.Current.Select(c => new Change<T>(ListChangeReason.Add, c)));

                var prev = x.Previous.ToList();
                var cur = x.Current.ToList();
                return new ChangeSet<T>(
                    prev
                        .ExceptBy(cur.Select(keySelector), keySelector)
                        .Select(item => new Change<T>(ListChangeReason.Remove, item))
                        .Concat(
                            cur
                                .ExceptBy(prev.Select(keySelector), keySelector)
                                .Select(item => new Change<T>(ListChangeReason.Add, item))));
            })
            .ToObservableCollection(disposable);
    }

    public static ReadOnlyObservableCollection<TTarget> ToObservableCollection<T, TKey, TTarget>(
        this IObservable<IEnumerable<T>> listObservable,
        Func<T, TKey> keySelector,
        Func<T, TTarget> selector,
        IDisposableDropoff disposable)
        where T : notnull
        where TTarget : notnull {
        return listObservable
            .ObserveOnGui()
            .Pairwise()
            .Select(x => {
                if (x.Current is null) return [];
                if (x.Previous is null) return new ChangeSet<T>(x.Current.Select(c => new Change<T>(ListChangeReason.Add, c)));

                var prev = x.Previous.ToList();
                var cur = x.Current.ToList();
                return new ChangeSet<T>(
                    prev
                        .ExceptBy(cur.Select(keySelector), keySelector)
                        .Select(item => new Change<T>(ListChangeReason.Remove, item))
                        .Concat(
                            cur
                                .ExceptBy(prev.Select(keySelector), keySelector)
                                .Select(item => new Change<T>(ListChangeReason.Add, item))));
            })
            .ToObservableCollection(selector, disposable);
    }

    public static IObservable<TResult> WrapInInProgressMarker<T, TResult>(
        this IObservable<T> changeSet,
        Func<IObservable<T>, IObservable<TResult>> wrapped,
        out IObservable<bool> isWorkingObservable) {

        var isWorking = new BehaviorSubject<bool>(false);
        isWorkingObservable = isWorking;

        return wrapped(changeSet.Do(_ => isWorking.OnNext(true)))
            .Do(_ => isWorking.OnNext(false));
    }

    /// <summary>
    /// Creates a binding to a readonly observable collection which is specified as an 'out' parameter.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="selector"></param>
    /// <param name="readOnlyObservableCollection">The resulting read only observable collection.</param>
    /// <param name="resetThreshold">The reset threshold.</param>
    /// <returns>A continuation of the source stream.</returns>
    public static IObservable<IChangeSet<TTarget>> Bind<T, TTarget>(
        this IObservable<IChangeSet<T>> source,
        Func<T, TTarget> selector,
        out ReadOnlyObservableCollection<TTarget> readOnlyObservableCollection,
        int resetThreshold = 25)
        where TTarget : notnull
        where T : notnull {
        ArgumentNullException.ThrowIfNull(source);

        var target = new ObservableCollectionExtended<TTarget>();
        var result = new ReadOnlyObservableCollection<TTarget>(target);
        var adaptor = new ObservableCollectionSelectorAdaptor<T, TTarget>(selector, target, resetThreshold);
        readOnlyObservableCollection = result;
        return source.Adapt(adaptor);
    }

    /// <summary>
    /// Creates a binding to a readonly observable collection which is specified as an 'out' parameter.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TObj"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="selector"></param>
    /// <param name="readOnlyObservableCollection">The resulting read only observable collection.</param>
    /// <param name="resetThreshold">The reset threshold.</param>
    /// <returns>A continuation of the source stream.</returns>
    public static IObservable<IChangeSet<TTarget>> Bind<TObj, TKey, TTarget>(
        this IObservable<IChangeSet<TObj, TKey>> source,
        Func<TObj, TKey, TTarget> selector,
        out ReadOnlyObservableCollection<TTarget> readOnlyObservableCollection,
        int resetThreshold = 25)
        where TKey : notnull
        where TTarget : notnull
        where TObj : notnull {
        ArgumentNullException.ThrowIfNull(source);

        var target = new ObservableCollectionExtended<TTarget>();
        var result = new ReadOnlyObservableCollection<TTarget>(target);
        var adaptor = new ObservableCollectionSelectorAdaptor<TObj, TKey, TTarget>(selector, target, resetThreshold);
        readOnlyObservableCollection = result;
        return source.Adapt(adaptor);
    }

    /// <summary>
    /// Injects a side effect into a change set observable.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="adaptor">The adaptor.</param>
    /// <returns>An observable which emits the change set.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// source
    /// or
    /// adaptor.
    /// </exception>
    public static IObservable<IChangeSet<TTarget>> Adapt<T, TTarget>(
        this IObservable<IChangeSet<T>> source,
        ObservableCollectionSelectorAdaptor<T, TTarget> adaptor)
        where TTarget : notnull
        where T : notnull {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(adaptor);

        return Observable.Create<IChangeSet<TTarget>>(
            observer => {
                var locker = new object();
                return source.Synchronize(locker)
                    .Select(adaptor.Adapt)
                    .SubscribeSafe(observer);
            });
    }

    /// <summary>
    /// Injects a side effect into a change set observable.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TObj"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="adaptor">The adaptor.</param>
    /// <returns>An observable which emits the change set.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// source
    /// or
    /// adaptor.
    /// </exception>
    public static IObservable<IChangeSet<TTarget>> Adapt<TObj, TKey, TTarget>(
        this IObservable<IChangeSet<TObj, TKey>> source,
        ObservableCollectionSelectorAdaptor<TObj, TKey, TTarget> adaptor)
        where TKey : notnull
        where TTarget : notnull
        where TObj : notnull {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(adaptor);

        return Observable.Create<IChangeSet<TTarget>>(
            observer => {
                var locker = new object();
                return source.Synchronize(locker)
                    .Select(adaptor.Adapt)
                    .SubscribeSafe(observer);
            });
    }

    public static IObservable<bool> Negate(this IObservable<bool> obs) => obs.Select(x => !x);
    public static IObservable<bool?> Negate(this IObservable<bool?> obs) => obs.Select(x => !x);

    public static IObservable<bool> WhereTrue(this IObservable<bool> obs) => obs.Where(x => x);
    public static IObservable<bool?> WhereTrue(this IObservable<bool?> obs) => obs.Where(x => x is true);
    public static IObservable<bool> WhereFalse(this IObservable<bool> obs) => obs.Where(x => !x);
    public static IObservable<bool?> WhereFalse(this IObservable<bool?> obs) => obs.Where(x => x is not true);
}
