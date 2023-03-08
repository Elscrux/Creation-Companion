using System.Collections.Concurrent;
using CreationEditor.Extension;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.FormLink;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Query;
using CreationEditor.Services.Notification;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Controller;

public class ReferenceController : IReferenceController {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IReferenceQuery _referenceQuery;
    private readonly INotificationService _notificationService;

    private MutableReferenceCache? _referenceCache;
    public IReferenceCache? ReferenceCache => _referenceCache;

    private readonly ConcurrentDictionary<FormKey, List<Action<Change<IFormLinkIdentifier>>>> _subscribers = new();

    public ReferenceController(
        IEditorEnvironment editorEnvironment,
        IReferenceQuery referenceQuery,
        INotificationService notificationService) {
        _editorEnvironment = editorEnvironment;
        _referenceQuery = referenceQuery;
        _notificationService = notificationService;

        _editorEnvironment.LoadOrderChanged
            .ObserveOnTaskpool()
            .Subscribe(_ => Init());

        // todo write back the current state of the mutable ref cache state - make sure to write new checksum of the SAVED file - entry point after plugin saved
    }

    private Task Init() {
        var linearNotifier = new ChainedNotifier(_notificationService, "Loading References");

        var immutableReferenceCache = new ImmutableReferenceCache(_referenceQuery, _editorEnvironment.LinkCache.PriorityOrder);
        _referenceCache = new MutableReferenceCache(_referenceQuery, _editorEnvironment.ActiveMod, immutableReferenceCache);

        linearNotifier.Stop();

        var modKeys = _editorEnvironment.LinkCache.PriorityOrder.Select(mod => mod.ModKey).ToList();
        foreach (var subscription in _subscribers.Keys.Where(formKey => !modKeys.Contains(formKey.ModKey))) {
            _subscribers.TryRemove(subscription, out _);
        }

        foreach (var (formKey, actions) in _subscribers) {
            if (!modKeys.Contains(formKey.ModKey)) continue;

            var references = _referenceCache.GetReferences(formKey, _editorEnvironment.LinkCache);
            var change = new Change<IFormLinkIdentifier>(ListChangeReason.AddRange, references);

            foreach (var action in actions) {
                action(change);
            }
        }

        return Task.CompletedTask;
    }

    public IDisposable GetRecord<TMajorRecordGetter>(TMajorRecordGetter record, out ReferencedRecord<TMajorRecordGetter> outReferencedRecord)
        where TMajorRecordGetter : IMajorRecordIdentifier {
        var recordFormKey = record.FormKey;
        var references = _referenceCache?.GetReferences(recordFormKey, _editorEnvironment.LinkCache);
        outReferencedRecord = new ReferencedRecord<TMajorRecordGetter>(record, references);

        var referencedRecord = outReferencedRecord;
        return SubscribeReferenceUpdate(recordFormKey, change => referencedRecord.References.Apply(change, FormLinkIdentifierEqualityComparer.Instance));
    }

    public void UpdateReferences<TMajorRecordGetter>(TMajorRecordGetter record, Action updateAction)
        where TMajorRecordGetter : IMajorRecordGetter {
        if (_referenceCache == null) throw new NotImplementedException("Reference update too early - implement properly");

        var before = record.EnumerateFormLinks().ToHashSet();

        updateAction();

        var after = record.EnumerateFormLinks().ToHashSet();

        // Add to the active mod's mutable reference cache if wasn't present before
        var recordIsNew = _referenceCache.AddRecord(record, after);

        var removedReferences = before.Except(after);
        var addedReferences = after.Except(before);

        foreach (var removed in removedReferences) {
            var removedFormKey = removed.FormKey;
            _referenceCache.RemoveReference(removedFormKey, record);

            if (!_subscribers.TryGetValue(removedFormKey, out var actions)) continue;

            var change = new Change<IFormLinkIdentifier>(ListChangeReason.Remove, record);
            foreach (var action in actions) {
                action(change);
            }
        }

        foreach (var added in addedReferences) {
            var addedFormKey = added.FormKey;
            if (!_referenceCache.AddReference(addedFormKey, record) && !recordIsNew) continue;
            if (!_subscribers.TryGetValue(addedFormKey, out var actions)) continue;

            var change = new Change<IFormLinkIdentifier>(ListChangeReason.Add, record);
            foreach (var action in actions) {
                action(change);
            }
        }
    }

    private IDisposable SubscribeReferenceUpdate(FormKey formKey, Action<Change<IFormLinkIdentifier>> action) {
        var actions = _subscribers.GetOrAdd(formKey);
        actions.Add(action);

        return new ReferenceSubscription(this, formKey, action);
    }

    private record ReferenceSubscription(ReferenceController ReferenceController, FormKey FormKey, Action<Change<IFormLinkIdentifier>> Action) : IDisposable {
        public void Dispose() {
            if (!ReferenceController._subscribers.TryGetValue(FormKey, out var actions)) return;

            actions.Remove(Action);

            if (actions.Count == 0) {
                ReferenceController._subscribers.TryRemove(FormKey, out _);
            }
        }
    }
}
