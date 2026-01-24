using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Models.GroupCollection;

/// <summary>
/// Groups items by their properties in a <see cref="ObservableCollection{T}"/>
/// </summary>
/// <typeparam name="T">Type of items</typeparam>
public sealed class GroupCollection<T> : IDisposable {
    private readonly IDisposable _sourceCollectionSubscription;
    private readonly List<Group<T>> _activeGroups = [];
    private readonly GroupInstance _topLevelGroup;

    public IObservableCollection<object> Items => _topLevelGroup.Items;

    public GroupCollection(ReadOnlyObservableCollection<T> source, params IEnumerable<Group<T>> groups) {
        _topLevelGroup = new GroupInstance(null!, new ObservableCollectionExtended<object>(source.OfType<object>()));

        foreach (var group in groups) {
            group.WhenAnyValue(x => x.IsGrouped)
                .DistinctUntilChanged()
                .Subscribe(grouped => {
                    if (grouped) {
                        _activeGroups.Add(group);

                        _topLevelGroup.GroupBy(group);
                    } else {
                        var level = _activeGroups.IndexOf(group);

                        _activeGroups.Remove(group);

                        _topLevelGroup.Ungroup<T>(level);
                    }
                })
                .DisposeWith(group);
        }

        _sourceCollectionSubscription = source.ObserveCollectionChanges()
            .Subscribe(x => {
                switch (x.EventArgs.Action) {
                    case NotifyCollectionChangedAction.Add:
                        if (x.EventArgs.NewItems is null) break;

                        _topLevelGroup.Add(x.EventArgs.NewItems.OfType<T>(), _activeGroups);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (x.EventArgs.OldItems is null) break;

                        _topLevelGroup.Remove(x.EventArgs.OldItems.OfType<T>());
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        if (x.EventArgs.OldItems is null || x.EventArgs.NewItems is null) break;

                        _topLevelGroup.Remove(x.EventArgs.OldItems.OfType<T>());
                        _topLevelGroup.Add(x.EventArgs.NewItems.OfType<T>(), _activeGroups);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _topLevelGroup.Clear();
                        if (x.Sender is IList list) {
                            _topLevelGroup.Add(list.OfType<T>(), _activeGroups);
                        }
                        break;
                }
            });
    }

    public void Dispose() => _sourceCollectionSubscription.Dispose();
}
