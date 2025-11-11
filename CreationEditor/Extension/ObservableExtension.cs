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
    extension<T>(IObservable<T> obs) {
        public IObservable<T> DoOnGuiAndSwitchBack(Action<T> onNext) {
            return obs
                .ObserveOnGui()
                .Do(onNext)
                .ObserveOnTaskpool();
        }
        public IObservable<T> DoOnGui(Action<T> onNext) {
            return obs
                .ObserveOnGui()
                .Do(onNext);
        }
        public IObservable<TResult> WrapInProgressMarker<TResult>(
            Func<IObservable<T>, IObservable<TResult>> wrapped,
            out IObservable<bool> isWorkingObservable) {

            var isWorking = new BehaviorSubject<bool>(false);
            isWorkingObservable = isWorking;

            return wrapped(obs.Do(_ => isWorking.OnNext(true)))
                .Do(_ => isWorking.OnNext(false));
        }
        public IObservable<T> ObserveOnGui() => obs.ObserveOn(RxApp.MainThreadScheduler);
        public IObservable<T> ObserveOnTaskpool() => obs.ObserveOn(RxApp.TaskpoolScheduler);
        public IObservable<T> ThrottleShort() => obs.Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler);
        public IObservable<T> ThrottleMedium() => obs.Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler);
        public IObservable<T> ThrottleLong() => obs.Throttle(TimeSpan.FromMilliseconds(700), RxApp.MainThreadScheduler);
        public IObservable<T> ThrottleVeryLong() =>
            obs.Throttle(TimeSpan.FromMilliseconds(1200), RxApp.MainThreadScheduler);
        public IObservable<T> UpdateWhenCollectionChanges(INotifyCollectionChanged collection) {
            return obs.CombineLatest(collection.WhenCollectionChanges(), (t, _) => t);
        }
    }

    extension(IObservable<bool> obs) {
        public IObservable<bool> Negate() => obs.Select(x => !x);
        public IObservable<bool> WhereTrue() => obs.Where(x => x);
        public IObservable<bool> WhereFalse() => obs.Where(x => !x);
    }

    extension(IObservable<bool?> obs) {
        public IObservable<bool?> Negate() => obs.Select(x => !x);
        public IObservable<bool?> WhereTrue() => obs.Where(x => x is true);
        public IObservable<bool?> WhereFalse() => obs.Where(x => x is not true);
    }

    extension(INotifyCollectionChanged source) {
        public IObservable<Unit> WhenCollectionChanges() =>
            source.ObserveCollectionChanges().Unit().StartWith(Unit.Default);
    }

    extension<T>(IObservable<IObservableCollection<T>> observableCollectionChanged)
        where T : notnull {
        public IDisposable SyncTo(ObservableCollection<T> observableCollection) {
            return observableCollectionChanged
                .Subscribe(observableCollection.ReplaceWith);
        }
    }

    extension(INotifyCollectionChanged source) {
        public IObservable<T> SelectWhenCollectionChanges<T>(Func<IObservable<T>> selector) {
            return source
                .WhenCollectionChanges()
                .Select(_ => selector())
                .Switch();
        }
    }

    /// <param name="observable">The source.</param>
    /// <typeparam name="TObj"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    extension<TObj, TKey>(IObservable<IChangeSet<TObj, TKey>> observable)
        where TObj : notnull
        where TKey : notnull {
        public ReadOnlyObservableCollection<TObj> ToObservableCollectionSync(
            IEnumerable<TObj> initial,
            IDisposableDropoff disposable) {
            var collection = new ObservableCollectionExtended<TObj>(initial);
            var adaptor = new ObservableCollectionAdaptor<TObj, TKey>();

            observable
                .Subscribe(changeSet => adaptor.Adapt(changeSet, collection))
                .DisposeWith(disposable);

            return new ReadOnlyObservableCollection<TObj>(collection);
        }
        public IObservableCollection<TObj> ToObservableCollection(IDisposableDropoff disposable) {
            var destination = new ObservableCollectionExtended<TObj>();

            observable
                .ObserveOnGui()
                .Bind(destination)
                .Subscribe()
                .DisposeWith(disposable);

            return destination;
        }
        /// <summary>
        /// Creates a binding to a readonly observable collection which is specified as an 'out' parameter.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="selector"></param>
        /// <param name="readOnlyObservableCollection">The resulting read only observable collection.</param>
        /// <param name="resetThreshold">The reset threshold.</param>
        /// <returns>A continuation of the source stream.</returns>
        public IObservable<IChangeSet<TTarget>> Bind<TTarget>(
            Func<TObj, TKey, TTarget> selector,
            out ReadOnlyObservableCollection<TTarget> readOnlyObservableCollection,
            int resetThreshold = 25)
            where TTarget : notnull {
            ArgumentNullException.ThrowIfNull(observable);

            var target = new ObservableCollectionExtended<TTarget>();
            var result = new ReadOnlyObservableCollection<TTarget>(target);
            var adaptor = new ObservableCollectionSelectorAdaptor<TObj, TKey, TTarget>(selector, target, resetThreshold);
            readOnlyObservableCollection = result;
            return observable.Adapt(adaptor);
        }
        /// <summary>
        /// Injects a side effect into a change set observable.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="adaptor">The adaptor.</param>
        /// <returns>An observable which emits the change set.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// source
        /// or
        /// adaptor.
        /// </exception>
        public IObservable<IChangeSet<TTarget>> Adapt<TTarget>(ObservableCollectionSelectorAdaptor<TObj, TKey, TTarget> adaptor)
            where TTarget : notnull {
            ArgumentNullException.ThrowIfNull(observable);
            ArgumentNullException.ThrowIfNull(adaptor);

            return Observable.Create<IChangeSet<TTarget>>(observer => {
                var locker = new object();
                return observable.Synchronize(locker)
                    .Select(adaptor.Adapt)
                    .SubscribeSafe(observer);
            });
        }
    }

    extension<TObj, TKey>(IObservable<ISortedChangeSet<TObj, TKey>> changeSet)
        where TObj : notnull
        where TKey : notnull {
        public IObservableCollection<TObj> ToObservableCollection(IDisposableDropoff disposable) {
            var destination = new ObservableCollectionExtended<TObj>();
            changeSet
                .ObserveOnGui()
                .Bind(destination)
                .Subscribe()
                .DisposeWith(disposable);

            return destination;
        }
        public ReadOnlyObservableCollection<TTarget> ToObservableCollection<TTarget>(
            Func<TObj, TKey, TTarget> selector,
            IDisposableDropoff disposable)
            where TTarget : notnull {
            changeSet
                .ObserveOnGui()
                .Bind(selector, out var readOnlyObservableCollection)
                .Subscribe()
                .DisposeWith(disposable);

            return readOnlyObservableCollection;
        }
        public ReadOnlyObservableCollection<TObj> ToObservableCollectionSync(
            IEnumerable<TObj> initial,
            IDisposableDropoff disposable) {
            var collection = new ObservableCollectionExtended<TObj>(initial);
            var adaptor = new SortedObservableCollectionAdaptor<TObj, TKey>();

            changeSet
                .Subscribe(sortedChangeSet => adaptor.Adapt(sortedChangeSet, collection))
                .DisposeWith(disposable);

            return new ReadOnlyObservableCollection<TObj>(collection);
        }
    }

    /// <param name="changeSet">The source.</param>
    /// <typeparam name="TObj">The type of the item.</typeparam>
    extension<TObj>(IObservable<IChangeSet<TObj>> changeSet)
        where TObj : notnull {
        public ReadOnlyObservableCollection<TObj> ToObservableCollection(IDisposableDropoff disposable) {
            changeSet
                .ObserveOnGui()
                .Bind(out var readOnlyObservableCollection)
                .Subscribe()
                .DisposeWith(disposable);

            return readOnlyObservableCollection;
        }
        public ReadOnlyObservableCollection<TTarget> ToObservableCollection<TTarget>(
            Func<TObj, TTarget> selector,
            IDisposableDropoff disposable)
            where TTarget : notnull {
            changeSet
                .ObserveOnGui()
                .Bind(selector, out var readOnlyObservableCollection)
                .Subscribe()
                .DisposeWith(disposable);

            return readOnlyObservableCollection;
        }
        /// <summary>
        /// Creates a binding to a readonly observable collection which is specified as an 'out' parameter.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="selector"></param>
        /// <param name="readOnlyObservableCollection">The resulting read only observable collection.</param>
        /// <param name="resetThreshold">The reset threshold.</param>
        /// <returns>A continuation of the source stream.</returns>
        public IObservable<IChangeSet<TTarget>> Bind<TTarget>(
            Func<TObj, TTarget> selector,
            out ReadOnlyObservableCollection<TTarget> readOnlyObservableCollection,
            int resetThreshold = 25)
            where TTarget : notnull {
            ArgumentNullException.ThrowIfNull(changeSet);

            var target = new ObservableCollectionExtended<TTarget>();
            var result = new ReadOnlyObservableCollection<TTarget>(target);
            var adaptor = new ObservableCollectionSelectorAdaptor<TObj, TTarget>(selector, target, resetThreshold);
            readOnlyObservableCollection = result;
            return changeSet.Adapt(adaptor);
        }
        /// <summary>
        /// Injects a side effect into a change set observable.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="adaptor">The adaptor.</param>
        /// <returns>An observable which emits the change set.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// source
        /// or
        /// adaptor.
        /// </exception>
        public IObservable<IChangeSet<TTarget>> Adapt<TTarget>(ObservableCollectionSelectorAdaptor<TObj, TTarget> adaptor)
            where TTarget : notnull {
            ArgumentNullException.ThrowIfNull(changeSet);
            ArgumentNullException.ThrowIfNull(adaptor);

            return Observable.Create<IChangeSet<TTarget>>(observer => {
                var locker = new object();
                return changeSet.Synchronize(locker)
                    .Select(adaptor.Adapt)
                    .SubscribeSafe(observer);
            });
        }
    }

    extension<T>(IObservable<IEnumerable<T>> listObservable)
        where T : notnull {
        public ReadOnlyObservableCollection<T> ToObservableCollection(IDisposableDropoff disposable) {
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
        public ReadOnlyObservableCollection<T> ToObservableCollection<TKey>(
            Func<T, TKey> keySelector,
            IDisposableDropoff disposable) {
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
        public ReadOnlyObservableCollection<TTarget> ToObservableCollection<TKey, TTarget>(
            Func<T, TKey> keySelector,
            Func<T, TTarget> selector,
            IDisposableDropoff disposable)
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
    }
}
