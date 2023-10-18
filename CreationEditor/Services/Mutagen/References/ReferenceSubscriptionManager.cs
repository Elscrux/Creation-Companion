using System.Collections.Concurrent;
using DynamicData;
using Noggog;
namespace CreationEditor.Services.Mutagen.References;

public sealed class ReferenceSubscriptionManager<TIdentifier, TSubscriber, TReference>
    where TIdentifier : notnull
    where TReference : notnull {
    private readonly object _lockObject = new();
    private readonly Func<TSubscriber, TIdentifier> _identifierSelector;
    private readonly Action<TSubscriber, Change<TReference>> _changeAction;

    private readonly ConcurrentDictionary<TIdentifier, List<ReferenceSubscription>> _identifierSubscriptions;

    public ReferenceSubscriptionManager(Action<TSubscriber, Change<TReference>> changeAction,
        Func<TSubscriber, TIdentifier> identifierSelector,
        IEqualityComparer<TIdentifier>? comparer = null) {
        _identifierSelector = identifierSelector;
        _changeAction = changeAction;
        _identifierSubscriptions = new ConcurrentDictionary<TIdentifier, List<ReferenceSubscription>>(comparer);
    }

    public IDisposable Register(TSubscriber subscriber) {
        var identifier = _identifierSelector(subscriber);
        var subscriptions = _identifierSubscriptions.GetOrAdd(identifier);

        var newSubscription = new ReferenceSubscription(this, identifier, subscriber);

        lock (_lockObject) subscriptions.Add(newSubscription);

        return newSubscription;
    }

    public void UnregisterAll() {
        _identifierSubscriptions.Clear();
    }

    public void Unregister(TIdentifier identifier, TSubscriber subscriber) {
        if (_identifierSubscriptions.TryGetValue(identifier, out var subscriptions)) {
            subscriptions.RemoveWhere(s => Equals(s.Subscriber, subscriber));
        }
    }

    public void UnregisterWhere(Predicate<TSubscriber> removePredicate) {
        var emptyIdents = new List<TIdentifier>();
        lock (_lockObject) {
            foreach (var (identifier, subscriptions) in _identifierSubscriptions) {
                subscriptions.RemoveWhere(subscription => removePredicate(subscription.Subscriber));

                if (subscriptions.Count == 0) {
                    emptyIdents.Add(identifier);
                }
            }
        }

        _identifierSubscriptions.Remove(emptyIdents);
    }

    public bool Change(TIdentifier identifier, Change<TReference> change) {
        if (!_identifierSubscriptions.TryGetValue(identifier, out var subscriptions)) return false;

        lock (_lockObject) {
            foreach (var subscription in subscriptions.ToArray()) {
                _changeAction(subscription.Subscriber, change);
            }
        }

        return true;
    }

    public void Change(Func<TIdentifier, Change<TReference>> changeSelector) {
        foreach (var (identifier, subscriptions) in _identifierSubscriptions) {
            var change = changeSelector(identifier);
            if (change is null) continue;

            lock (_lockObject) {
                foreach (var subscription in subscriptions) {
                    _changeAction(subscription.Subscriber, change);
                }
            }
        }
    }

    public sealed record ReferenceSubscription(
        ReferenceSubscriptionManager<TIdentifier, TSubscriber, TReference> ReferenceSubscriptionManager,
        TIdentifier Identifier,
        TSubscriber Subscriber) : IDisposable {
        public void Dispose() {
            if (!ReferenceSubscriptionManager._identifierSubscriptions.TryGetValue(Identifier, out var subscriptions)) return;

            lock (ReferenceSubscriptionManager._lockObject) {
                subscriptions.Remove(this);

                if (subscriptions.Count == 0) {
                    ReferenceSubscriptionManager._identifierSubscriptions.TryRemove(Identifier, out _);
                }
            }
        }
    }
}
