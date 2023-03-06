using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Extension;

// Mostly copied from Noggog.CSharpExt
public static class ObservableExtension {
    public static IObservable<T> ObserveOnGui<T>(this IObservable<T> obs) => obs.ObserveOn(RxApp.MainThreadScheduler);
    public static IObservable<T> ObserveOnTaskpool<T>(this IObservable<T> obs) => obs.ObserveOn(RxApp.TaskpoolScheduler);

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

    public static IObservableCollection<TObj> ToObservableCollection<TObj, TKey>(
        this IObservable<IChangeSet<TObj, TKey>> changeSet,
        IDisposableDropoff disposable)
        where TKey : notnull {
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
        where TKey : notnull {
        var destination = new ObservableCollectionExtended<TObj>();
        changeSet
            .ObserveOnGui()
            .Bind(destination)
            .Subscribe()
            .DisposeWith(disposable);
        
        return destination;
    }

    public static ReadOnlyObservableCollection<TObj> ToObservableCollection<TObj>(
        this IObservable<IChangeSet<TObj>> changeSet,
        IDisposableDropoff disposable) {
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
        IDisposableDropoff disposable) {
        changeSet
            .ObserveOnGui()
            .Bind(selector, out var readOnlyObservableCollection)
            .Subscribe()
            .DisposeWith(disposable);

        return readOnlyObservableCollection;
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
        int resetThreshold = 25) {
        if (source is null) {
            throw new ArgumentNullException(nameof(source));
        }

        var target = new ObservableCollectionExtended<TTarget>();
        var result = new ReadOnlyObservableCollection<TTarget>(target);
        var adaptor = new ObservableCollectionSelectorAdaptor<T, TTarget>(selector, target, resetThreshold);
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
        ObservableCollectionSelectorAdaptor<T, TTarget> adaptor) {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (adaptor is null) throw new ArgumentNullException(nameof(adaptor));

        return Observable.Create<IChangeSet<TTarget>>(
            observer => {
                var locker = new object();
                return source.Synchronize(locker)
                    .Select(adaptor.Adapt)
                    .SubscribeSafe(observer);
            });
    }
}