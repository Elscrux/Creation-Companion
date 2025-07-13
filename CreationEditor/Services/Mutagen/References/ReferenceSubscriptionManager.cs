using System.Collections.Concurrent;
using DynamicData;
using Noggog;
namespace CreationEditor.Services.Mutagen.References;

public sealed class ReferenceSubscriptionManager<TLink, TReference, TSubscriber>(
    Predicate<TSubscriber> isOutdated,
    Action<TSubscriber, Change<TReference>> changeAction,
    Action<TSubscriber, IReadOnlyList<TReference>> addAllAction,
    Func<TSubscriber, TLink> identifierSelector,
    IEqualityComparer<TLink>? comparer = null)
    where TLink : notnull
    where TReference : notnull {

    private readonly Lock _lockObject = new();
    private readonly ConcurrentDictionary<TLink, ConcurrentDictionary<ReferenceSubscription, byte>> _identifierSubscriptions = new(comparer);

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

    public void Unregister(TLink link, TSubscriber subscriber) {
        if (!_identifierSubscriptions.TryGetValue(link, out var subscriptions)) return;

        var unregisteredKeys = subscriptions.Keys.Where(s => Equals(s.Subscriber, subscriber)).ToList();
        subscriptions.Remove(unregisteredKeys);
    }

    public void UnregisterOutdated() {
        var emptyIdents = new List<TLink>();
        foreach (var (identifier, subscriptions) in _identifierSubscriptions) {
            var unregisteredKeys = subscriptions.Keys.Where(s => isOutdated(s.Subscriber)).ToList();
            subscriptions.Remove(unregisteredKeys);

            if (subscriptions.IsEmpty) {
                emptyIdents.Add(identifier);
            }
        }

        _identifierSubscriptions.Remove(emptyIdents);
    }

    public bool Update(TLink link, Change<TReference> change) {
        if (!_identifierSubscriptions.TryGetValue(link, out var subscriptions)) return false;
        if (subscriptions.IsEmpty) return true;

        lock (_lockObject) {
            foreach (var subscription in subscriptions) {
                changeAction(subscription.Key.Subscriber, change);
            }
        }

        return true;
    }

    public void UpdateAll(Func<TLink, IEnumerable<TReference>> newReferencesSelector) {
        foreach (var (identifier, subscriptions) in _identifierSubscriptions) {
            if (subscriptions.IsEmpty) continue;

            var references = newReferencesSelector(identifier).ToList();

            foreach (var subscription in subscriptions) {
                addAllAction(subscription.Key.Subscriber, references);
            }
        }
    }

    public sealed record ReferenceSubscription(
        ReferenceSubscriptionManager<TLink, TReference, TSubscriber> ReferenceSubscriptionManager,
        TLink Link,
        TSubscriber Subscriber) : IDisposable {
        public void Dispose() {
            if (!ReferenceSubscriptionManager._identifierSubscriptions.TryGetValue(Link, out var subscriptions)) return;

            subscriptions.TryRemove(this, out _);

            if (subscriptions.IsEmpty) {
                ReferenceSubscriptionManager._identifierSubscriptions.TryRemove(Link, out _);
            }
        }
    }
}
