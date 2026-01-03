using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Query;
using CreationEditor.Services.Notification;
using DynamicData;
namespace CreationEditor.Services.Mutagen.References;

public sealed class ReferenceController<TSource, TSourceElement, TCache, TLink, TReference, TSubscriber>
    where TSource : notnull
    where TSourceElement : notnull
    where TCache : IReferenceCache<TCache, TLink, TReference>
    where TLink : notnull
    where TReference : notnull
    where TSubscriber : IReferenced {
    private readonly CompositeDisposable _disposables = new();
    private readonly INotificationService _notificationService;
    private readonly IReferenceUpdateTrigger<TSource, TSourceElement, TCache, TLink, TReference, TSubscriber> _updateTrigger;
    private readonly IReferenceCacheController<TSource, TCache, TLink, TReference> _cacheController;
    private readonly IReferenceQueryConfig<TSource, TSourceElement, TCache, TLink> _queryConfig;
    private readonly ReferenceSubscriptionManager<TLink, TReference, TSubscriber> _referenceSubscriptionManager;

    private readonly ConcurrentQueue<TSourceElement> _pendingCreations = new();
    private readonly ConcurrentQueue<TSourceElement> _pendingDeletions = new();

    private readonly ConcurrentDictionary<TSource, TCache> _caches;

    private readonly BehaviorSubject<bool> _isLoading = new(true);
    public IObservable<bool> IsLoading => _isLoading;
    public string Name => _queryConfig.Name;

    public ReferenceController(
        INotificationService notificationService,
        IReferenceUpdateTrigger<TSource, TSourceElement, TCache, TLink, TReference, TSubscriber> updateTrigger,
        IReferenceCacheController<TSource, TCache, TLink, TReference> cacheController,
        IReferenceQueryConfig<TSource, TSourceElement, TCache, TLink> queryConfig,
        ReferenceSubscriptionManager<TLink, TReference, TSubscriber> referenceSubscriptionManager) {
        _updateTrigger = updateTrigger;
        _cacheController = cacheController;
        _queryConfig = queryConfig;
        _referenceSubscriptionManager = referenceSubscriptionManager;
        _notificationService = notificationService;

        _caches = new ConcurrentDictionary<TSource, TCache>([], _queryConfig.EqualityComparer);
        _updateTrigger.SetupSubscriptions(this, _disposables);
    }

    public async Task UpdateSources(IReadOnlyList<TSource> sources, CancellationToken cancellationToken) {
        if (cancellationToken.IsCancellationRequested) return;

        _isLoading.OnNext(true);

        // Remove references from sources that are no longer present
        foreach (var source in _caches.Keys.Where(source => !sources.Contains(source, _queryConfig.EqualityComparer))) {
            _caches.TryRemove(source, out _);
        }

        _referenceSubscriptionManager.UnregisterOutdated();

        // New sources that are not yet cached
        var countingNotifier = new CountingNotifier(_notificationService, $"Loading {_queryConfig.Name} References ", sources.Count);
        var caches = sources
            .Where(source => !_caches.ContainsKey(source))
            .ToDictionary(source => Task.Run(() => _queryConfig.BuildCache(source), cancellationToken), source => source);

        var cachesCount = caches.Count;
        for (var i = 0; i < cachesCount; i++) {
            if (cancellationToken.IsCancellationRequested) break;

            var task = await Task.WhenAny(caches.Keys);
            var source = caches[task];
            _caches.TryAdd(source, task.Result);
            caches.Remove(task);
            countingNotifier.NextStep();
        }

        countingNotifier.Stop();

        // Update existing subscriptions
        _referenceSubscriptionManager.UpdateAll(link => _cacheController.GetReferences(_caches, link));

        // Handle previous reference creations and deletions  while the reference cache wasn't initialized
        while (_pendingCreations.TryDequeue(out var record)) RegisterCreation(record);
        while (_pendingDeletions.TryDequeue(out var record)) RegisterDeletion(record);

        _isLoading.OnNext(false);
    }

    public Action<TSourceElement> RegisterUpdate(TSourceElement old) {
        var cache = GetCacheFor(old);
        if (cache is null) return _ => {};

        // Collect the references before and after the update
        var oldReference = _updateTrigger.ToReference(old);
        var oldLinks = _queryConfig.CanGetLinksFromDeletedElement
            ? _queryConfig.GetLinks(old)
            : _cacheController.GetLinks(_caches.Values, oldReference);
        var before = oldLinks.ToHashSet();

        return updated => {
            var after = _queryConfig.GetLinks(updated).ToHashSet();

            // Calculate the diff
            var removedReferences = before.Except(after);
            var addedReferences = after.Except(before);

            var updatedReference = _updateTrigger.ToReference(updated);

            // Remove the record from its former references
            RemoveLinks(cache, updatedReference, removedReferences);

            // Add the record to its new references
            AddLinks(cache, updatedReference, addedReferences);
        };
    }

    public void RegisterCreation(TSourceElement created) {
        var cache = GetCacheFor(created);
        if (cache is null) {
            _pendingCreations.Enqueue(created);
            return;
        }

        var linksToAdd = _queryConfig.GetLinks(created);
        AddLinks(cache, _updateTrigger.ToReference(created), linksToAdd);
    }

    public void RegisterDeletion(TSourceElement deleted) {
        var cache = GetCacheFor(deleted);
        if (cache is null) {
            _pendingDeletions.Enqueue(deleted);
            return;
        }

        var oldReference = _updateTrigger.ToReference(deleted);
        var linksToRemove = _queryConfig.CanGetLinksFromDeletedElement
            ? _queryConfig.GetLinks(deleted)
            : _cacheController.GetLinks(_caches.Values, oldReference);
        RemoveLinks(cache, oldReference, linksToRemove);
    }

    public IEnumerable<TReference> GetReferences(TLink link) {
        return _cacheController.GetReferences(_caches, link);
    }

    public IEnumerable<TLink> GetLinks(TSourceElement element) {
        return _queryConfig.GetLinks(element);
    }

    private void AddLinks(
        TCache cache,
        TReference newReference,
        IEnumerable<TLink> links) {
        var linksToAdd = links.ToArray();
        _cacheController.AddLink(cache, newReference, linksToAdd);
        foreach (var link in linksToAdd) {
            var change = new Change<TReference>(ListChangeReason.Add, newReference);
            _referenceSubscriptionManager.Update(link, change);
        }
    }

    private void RemoveLinks(
        TCache cache,
        TReference oldReference,
        IEnumerable<TLink> links) {
        var linksToRemove = links.ToArray();
        _cacheController.RemoveLink(cache, oldReference, linksToRemove);
        foreach (var link in linksToRemove) {
            var change = new Change<TReference>(ListChangeReason.Remove, oldReference);
            _referenceSubscriptionManager.Update(link, change);
        }
    }

    private TCache? GetCacheFor(TSourceElement reference) {
        var source = _updateTrigger.GetSourceFor(reference);
        if (source is null) return default;

        return _caches.GetValueOrDefault(source);
    }
}
