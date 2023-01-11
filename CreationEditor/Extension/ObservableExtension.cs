using System.Collections.ObjectModel;
using System.Reactive.Linq;
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
}
