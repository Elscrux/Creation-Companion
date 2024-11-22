using System.Collections.Concurrent;
using DynamicData;
using Noggog;
namespace CreationEditor.Services.Mutagen.References;

public sealed class ReferenceSubscriptionManager<TIdentifier, TSubscriber, TReference>(
    Action<TSubscriber, Change<TReference>> changeAction,
    Action<TSubscriber, IReadOnlyList<TReference>> changeAllAction,
    Func<TSubscriber, TIdentifier> identifierSelector,
    IEqualityComparer<TIdentifier>? comparer = null)
    where TIdentifier : notnull
    where TReference : notnull {

    private readonly Lock _lockObject = new();
    private readonly ConcurrentDictionary<TIdentifier, ConcurrentDictionary<ReferenceSubscription, byte>> _identifierSubscriptions = new(comparer);

    public IDisposable Register(TSubscriber subscriber) {
        var identifier = identifierSelector(subscriber);
        var subscriptions = _identifierSubscriptions.GetOrAdd(identifier);

        var newSubscription = new ReferenceSubscription(this, identifier, subscriber);

        subscriptions.TryAdd(newSubscription, byte.MaxValue);

        return newSubscription;
    }

    public void UnregisterAll() {
        _identifierSubscriptions.Clear();
    }

    public void Unregister(TIdentifier identifier, TSubscriber subscriber) {
        if (!_identifierSubscriptions.TryGetValue(identifier, out var subscriptions)) return;

        var unregisteredKeys = subscriptions.Keys.Where(s => Equals(s.Subscriber, subscriber)).ToList();
        subscriptions.Remove(unregisteredKeys);
    }

    public void UnregisterWhere(Predicate<TSubscriber> removePredicate) {
        var emptyIdents = new List<TIdentifier>();
        foreach (var (identifier, subscriptions) in _identifierSubscriptions) {
            var unregisteredKeys = subscriptions.Keys.Where(s => removePredicate(s.Subscriber)).ToList();
            subscriptions.Remove(unregisteredKeys);

            if (subscriptions.IsEmpty) {
                emptyIdents.Add(identifier);
            }
        }

        _identifierSubscriptions.Remove(emptyIdents);
    }

    public bool Update(TIdentifier identifier, Change<TReference> change) {
        if (!_identifierSubscriptions.TryGetValue(identifier, out var subscriptions)) return false;
        if (subscriptions.IsEmpty) return true;

        lock (_lockObject) {
            foreach (var subscription in subscriptions) {
                changeAction(subscription.Key.Subscriber, change);
            }
        }

        return true;
    }

    public void UpdateAll(Func<TIdentifier, IEnumerable<TReference>> newReferencesSelector) {
        foreach (var (identifier, subscriptions) in _identifierSubscriptions) {
            if (subscriptions.IsEmpty) continue;

            var references = newReferencesSelector(identifier).ToList();

            foreach (var subscription in subscriptions) {
                changeAllAction(subscription.Key.Subscriber, references);
            }
        }
    }

    public sealed record ReferenceSubscription(
        ReferenceSubscriptionManager<TIdentifier, TSubscriber, TReference> ReferenceSubscriptionManager,
        TIdentifier Identifier,
        TSubscriber Subscriber) : IDisposable {
        public void Dispose() {
            if (!ReferenceSubscriptionManager._identifierSubscriptions.TryGetValue(Identifier, out var subscriptions)) return;

            subscriptions.TryRemove(this, out _);

            if (subscriptions.IsEmpty) {
                ReferenceSubscriptionManager._identifierSubscriptions.TryRemove(Identifier, out _);
            }
        }
    }
}
