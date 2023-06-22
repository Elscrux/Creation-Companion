using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.FormLink;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record.Cache;
using CreationEditor.Services.Mutagen.References.Record.Query;
using CreationEditor.Services.Notification;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Record.Controller;

// todo rename to record reference controller
public sealed class RecordReferenceController : IRecordReferenceController, IDisposable {
    private readonly CompositeDisposable _disposable = new();
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IReferenceQuery _referenceQuery;
    private readonly INotificationService _notificationService;

    private readonly ConcurrentQueue<IMajorRecordGetter> _recordCreations = new();
    private readonly ConcurrentQueue<IMajorRecordGetter> _recordDeletions = new();

    private MutableReferenceCache? _referenceCache;
    public IReferenceCache? ReferenceCache => _referenceCache;

    private readonly BehaviorSubject<bool> _isLoading = new(true);
    public IObservable<bool> IsLoading => _isLoading;

    private readonly ReferenceSubscriptionManager<FormKey, IReferencedRecord, IFormLinkIdentifier> _referenceSubscriptionManager
        = new((record, change) => record.References.Apply(change, FormLinkIdentifierEqualityComparer.Instance),
            (record => record.FormKey));

    public RecordReferenceController(
        IRecordController recordController,
        IEditorEnvironment editorEnvironment,
        IReferenceQuery referenceQuery,
        INotificationService notificationService) {
        _editorEnvironment = editorEnvironment;
        _referenceQuery = referenceQuery;
        _notificationService = notificationService;

        _editorEnvironment.LoadOrderChanged
            .ObserveOnTaskpool()
            .Subscribe(Init)
            .DisposeWith(_disposable);

        recordController.RecordChangedDiff.Subscribe(RegisterUpdate);
        recordController.RecordCreated.Subscribe(RegisterCreation);
        recordController.RecordDeleted.Subscribe(RegisterDeletion);
        // todo write back the current state of the mutable ref cache state - make sure to write new checksum of the SAVED file - entry point after plugin saved
    }

    public void Dispose() => _disposable.Dispose();

    private Task Init() {
        _isLoading.OnNext(true);

        using var linearNotifier = new ChainedNotifier(_notificationService, "Loading Record References");

        var immutableReferenceCache = new ImmutableReferenceCache(_referenceQuery, _editorEnvironment.LinkCache.PriorityOrder);
        _referenceCache = new MutableReferenceCache(_referenceQuery, _editorEnvironment.ActiveMod, immutableReferenceCache);

        linearNotifier.Stop();

        // Remove references from unloaded mods
        var modKeys = _editorEnvironment.LinkCache.PriorityOrder.Select(mod => mod.ModKey).ToList();
        _referenceSubscriptionManager.UnregisterWhere(referencedRecord => !modKeys.Contains(referencedRecord.FormKey.ModKey));

        // Change existing subscriptions
        _referenceSubscriptionManager.Change(formKey => {
            var references = _referenceCache.GetReferences(formKey, _editorEnvironment.LinkCache);

            return new Change<IFormLinkIdentifier>(ListChangeReason.AddRange, references);
        });

        // Handle previous record creations and deletions  while the reference cache wasn't initialized
        while (_recordCreations.TryDequeue(out var record)) RegisterCreation(record);
        while (_recordDeletions.TryDequeue(out var record)) RegisterDeletion(record);

        _isLoading.OnNext(false);
        return Task.CompletedTask;
    }

    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey) {
        return _referenceCache?.GetReferences(formKey, _editorEnvironment.LinkCache) ?? Array.Empty<IFormLinkIdentifier>();
    }

    public IDisposable GetReferencedRecord<TMajorRecordGetter>(TMajorRecordGetter record, out IReferencedRecord<TMajorRecordGetter> outReferencedRecord)
        where TMajorRecordGetter : IMajorRecordGetter {
        var references = _referenceCache?.GetReferences(record.FormKey, _editorEnvironment.LinkCache);
        outReferencedRecord = new ReferencedRecord<TMajorRecordGetter>(record, references);

        return _referenceSubscriptionManager.Register(outReferencedRecord);
    }

    public Action<IMajorRecordGetter> RegisterUpdate(IMajorRecordGetter record) {
        if (_referenceCache is null) return _ => {};

        // Collect the references before the update
        var before = record.EnumerateFormLinks().Select(x => x.FormKey).ToHashSet();

        return newRecord => {
            var after = newRecord.EnumerateFormLinks().Select(x => x.FormKey).ToHashSet();

            // Calculate the diff
            var removedReferences = before.Except(after);
            var addedReferences = after.Except(before);

            // Remove the record from its former references
            RemoveRecordReferences(_referenceCache, newRecord, removedReferences);

            // Add the record to its new references
            // Add to the active mod's mutable reference cache if wasn't present before
            var recordIsNew = _referenceCache.AddRecord(newRecord);
            AddRecordReferences(_referenceCache, newRecord, addedReferences);
        };
    }

    public void RegisterCreation(IMajorRecordGetter record) {
        if (_referenceCache is null) {
            _recordCreations.Enqueue(record);
            return;
        }

        _referenceCache.AddRecord(record);
        AddRecordReferences(_referenceCache, record, record.EnumerateFormLinks().Select(x => x.FormKey));
    }

    public void RegisterDeletion(IMajorRecordGetter record) {
        if (_referenceCache is null) {
            _recordDeletions.Enqueue(record);
            return;
        }

        RemoveRecordReferences(_referenceCache, record, record.EnumerateFormLinks().Select(x => x.FormKey));
    }

    private void AddRecordReferences(MutableReferenceCache cache, IMajorRecordGetter record, IEnumerable<FormKey> references) {
        foreach (var reference in references) {
            if (!cache.AddReference(reference, record)) continue;

            var change = new Change<IFormLinkIdentifier>(ListChangeReason.Add, record);
            _referenceSubscriptionManager.Change(reference, change);
        }
    }

    private void RemoveRecordReferences(MutableReferenceCache cache, IMajorRecordGetter record, IEnumerable<FormKey> references) {
        foreach (var reference in references) {
            if (cache.RemoveReference(reference, record)) continue;

            var change = new Change<IFormLinkIdentifier>(ListChangeReason.Remove, record);
            _referenceSubscriptionManager.Change(reference, change);
        }
    }
}
