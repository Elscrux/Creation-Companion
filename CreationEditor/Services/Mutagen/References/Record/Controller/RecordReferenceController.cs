using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.FormLink;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record.Cache;
using CreationEditor.Services.Notification;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Record.Controller;

public sealed class RecordReferenceController : IRecordReferenceController, IDisposable {
    private readonly CompositeDisposable _disposable = new();
    private readonly ILogger _logger;
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IRecordReferenceCacheFactory _recordReferenceCacheFactory;
    private readonly INotificationService _notificationService;

    private readonly ConcurrentQueue<RecordModPair> _recordCreations = new();
    private readonly ConcurrentQueue<RecordModPair> _recordDeletions = new();

    private MutableRecordReferenceCache? _referenceCache;
    public IRecordReferenceCache? ReferenceCache => _referenceCache;

    private readonly BehaviorSubject<bool> _isLoading = new(true);
    public IObservable<bool> IsLoading => _isLoading;

    private readonly ReferenceSubscriptionManager<FormKey, IReferencedRecord, IFormLinkIdentifier> _referenceSubscriptionManager
        = new((record, change) => record.References.Apply(change, FormLinkIdentifierEqualityComparer.Instance),
            (record, newData) => record.References.ReplaceWith(newData),
            record => record.FormKey);

    public RecordReferenceController(
        ILogger logger,
        IRecordController recordController,
        IEditorEnvironment editorEnvironment,
        IRecordReferenceCacheFactory recordReferenceCacheFactory,
        INotificationService notificationService) {
        _logger = logger;
        _editorEnvironment = editorEnvironment;
        _recordReferenceCacheFactory = recordReferenceCacheFactory;
        _notificationService = notificationService;

        _editorEnvironment.LoadOrderChanged
            .ObserveOnTaskpool()
            .Subscribe(Init)
            .DisposeWith(_disposable);

        recordController.RecordChangedDiff.Subscribe(RegisterUpdate);
        recordController.RecordCreated.Subscribe(RegisterCreation);
        recordController.RecordDeleted.Subscribe(RegisterDeletion);
        // todo write back the current state of the mutable ref cache state - make sure to write new hash of the SAVED file - entry point after plugin saved
    }

    public void Dispose() => _disposable.Dispose();

    private async Task Init() {
        _isLoading.OnNext(true);

        using var linearNotifier = new ChainedNotifier(_notificationService, "Loading Record References");

        var mutableMods = _editorEnvironment.LinkCache.PriorityOrder.OfType<IMod>().ToList();
        _referenceCache = await _recordReferenceCacheFactory.GetMutableRecordReferenceCache(mutableMods, _editorEnvironment.LinkCache.PriorityOrder);

        linearNotifier.Stop();

        // Remove references from unloaded mods
        var modKeys = _editorEnvironment.LinkCache.PriorityOrder.Select(mod => mod.ModKey).ToList();
        _referenceSubscriptionManager.UnregisterWhere(referencedRecord => !modKeys.Contains(referencedRecord.FormKey.ModKey));

        // Change existing subscriptions
        _referenceSubscriptionManager.UpdateAll(formKey => _referenceCache.GetReferences(formKey, _editorEnvironment.LinkCache));

        // Handle previous record creations and deletions  while the reference cache wasn't initialized
        while (_recordCreations.TryDequeue(out var record)) RegisterCreation(record);
        while (_recordDeletions.TryDequeue(out var record)) RegisterDeletion(record);

        _isLoading.OnNext(false);

        _logger.Here().Information("Loaded Record References for {Count} Mods", modKeys.Count);
    }

    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey) {
        return _referenceCache?.GetReferences(formKey, _editorEnvironment.LinkCache) ?? [];
    }

    public IDisposable GetReferencedRecord<TMajorRecordGetter>(TMajorRecordGetter record, out IReferencedRecord<TMajorRecordGetter> outReferencedRecord)
        where TMajorRecordGetter : IMajorRecordGetter {
        var references = _referenceCache?.GetReferences(record.FormKey, _editorEnvironment.LinkCache);
        outReferencedRecord = new ReferencedRecord<TMajorRecordGetter>(record, references);

        return _referenceSubscriptionManager.Register(outReferencedRecord);
    }

    public Action<RecordModPair> RegisterUpdate(RecordModPair old) {
        if (_referenceCache is null) return _ => {};

        // Collect the references before the update
        var before = old.Record.EnumerateFormLinks().Select(x => x.FormKey).ToHashSet();

        return updated => {
            var after = updated.Record.EnumerateFormLinks().Select(x => x.FormKey).ToHashSet();

            // Calculate the diff
            var removedReferences = before.Except(after);
            var addedReferences = after.Except(before);

            // Remove the record from its former references
            RemoveRecordReferences(_referenceCache, updated.Record, updated.Mod, removedReferences);

            // Add the record to its new references
            // Add to the active mod's mutable reference cache if wasn't present before
            var recordIsNew = _referenceCache.AddRecord(updated.Mod, updated.Record);
            AddRecordReferences(_referenceCache, updated.Record, updated.Mod, addedReferences);
        };
    }

    public void RegisterCreation(RecordModPair pair) {
        if (_referenceCache is null) {
            _recordCreations.Enqueue(pair);
            return;
        }

        _referenceCache.AddRecord(pair.Mod, pair.Record);
        AddRecordReferences(_referenceCache, pair.Record, pair.Mod, pair.Record.EnumerateFormLinks().Select(x => x.FormKey));
    }

    public void RegisterDeletion(RecordModPair pair) {
        if (_referenceCache is null) {
            _recordDeletions.Enqueue(pair);
            return;
        }

        RemoveRecordReferences(_referenceCache, pair.Record, pair.Mod, pair.Record.EnumerateFormLinks().Select(x => x.FormKey));
    }

    private void AddRecordReferences(MutableRecordReferenceCache cache, IMajorRecordGetter record, IMod mod, IEnumerable<FormKey> references) {
        foreach (var reference in references) {
            if (!cache.AddReference(mod, reference, record)) continue;

            var change = new Change<IFormLinkIdentifier>(ListChangeReason.Add, record);
            _referenceSubscriptionManager.Update(reference, change);
        }
    }

    private void RemoveRecordReferences(MutableRecordReferenceCache cache, IMajorRecordGetter record, IMod mod, IEnumerable<FormKey> references) {
        foreach (var reference in references) {
            if (cache.RemoveReference(mod, reference, record)) continue;

            var change = new Change<IFormLinkIdentifier>(ListChangeReason.Remove, record);
            _referenceSubscriptionManager.Update(reference, change);
        }
    }
}
