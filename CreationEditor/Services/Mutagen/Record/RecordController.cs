using System.Reactive.Subjects;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Record;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
namespace CreationEditor.Services.Mutagen.Record;

public sealed class RecordController<TMod, TModGetter> : IRecordController
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {
    private readonly ILogger _logger;
    private readonly IEditorEnvironment<TMod, TModGetter> _editorEnvironment;

    private readonly Subject<IMajorRecordGetter> _recordChanged = new();
    public IObservable<IMajorRecordGetter> RecordChanged => _recordChanged;

    private readonly Subject<UpdateAction<IMajorRecordGetter>> _recordChangedDiff = new();
    public JoinedObservable<IMajorRecordGetter> RecordChangedDiff { get; }

    private readonly Subject<IMajorRecordGetter> _recordCreated = new();
    public IObservable<IMajorRecordGetter> RecordCreated => _recordCreated;

    private readonly Subject<IMajorRecordGetter> _recordDeleted = new();
    public IObservable<IMajorRecordGetter> RecordDeleted => _recordDeleted;

    public RecordController(
        ILogger logger,
        IEditorEnvironment<TMod, TModGetter> editorEnvironment) {
        _logger = logger;
        _editorEnvironment = editorEnvironment;
        RecordChangedDiff = new JoinedObservable<IMajorRecordGetter>(_recordChangedDiff);
    }

    public IMajorRecord CreateRecord(System.Type type) {
        var group = _editorEnvironment.ActiveMod.GetTopLevelGroup(type);
        var record = group.AddNew(_editorEnvironment.ActiveMod.GetNextFormKey());

        _logger.Here().Verbose("Creating new record {Record} of type {Type} in {Mod}",
            record, type, _editorEnvironment.ActiveMod.ModKey);

        _recordCreated.OnNext(record);

        return record;
    }

    public TMajorRecord CreateRecord<TMajorRecord, TMajorRecordGetter>()
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter {
        var group = _editorEnvironment.ActiveMod.GetTopLevelGroup<TMajorRecord>();
        var record = group.AddNew(_editorEnvironment.ActiveMod.GetNextFormKey());

        _logger.Here().Verbose("Creating new record {Record} of type {Type} in {Mod}",
            record, typeof(TMajorRecord), _editorEnvironment.ActiveMod.ModKey);

        _recordCreated.OnNext(record);

        return record;
    }

    public TMajorRecord DuplicateRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter {
        var resolveContext = _editorEnvironment.LinkCache.ResolveContext<TMajorRecord, TMajorRecordGetter>(record.FormKey);
        var duplicate = resolveContext.DuplicateIntoAsNewRecord(_editorEnvironment.ActiveMod);

        _logger.Here().Verbose("Creating new record {Duplicate} by duplicating {Record} in {Mod}",
            duplicate, record, _editorEnvironment.ActiveMod.ModKey);

        _recordCreated.OnNext(duplicate);

        return duplicate;
    }

    public IMajorRecord DuplicateRecord(IMajorRecordGetter record) {
        var resolveContext = _editorEnvironment.LinkCache.ResolveContext(record.FormKey, record.Registration.GetterType);
        var duplicate = resolveContext.DuplicateIntoAsNewRecord(_editorEnvironment.ActiveMod);

        _logger.Here().Verbose("Creating new record {Duplicate} by duplicating {Record} in {Mod}",
            duplicate, record, _editorEnvironment.ActiveMod.ModKey);

        _recordCreated.OnNext(duplicate);

        return duplicate;
    }

    public void DeleteRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter {
        var newOverride = GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(record);
        newOverride.IsDeleted = true;

        _logger.Here().Verbose("Deleting record {Record} from {Mod}",
            record, _editorEnvironment.ActiveMod);

        _recordDeleted.OnNext(newOverride);
    }

    public void DeleteRecord(IMajorRecordGetter record) {
        var newOverride = GetOrAddOverride(record);
        newOverride.IsDeleted = true;

        _logger.Here().Verbose("Deleting record {Record} from {Mod}",
            record, _editorEnvironment.ActiveMod);

        _recordDeleted.OnNext(newOverride);
    }

    public TMajorRecord GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter {
        if (!_editorEnvironment.LinkCache.TryResolveContext<TMajorRecord, TMajorRecordGetter>(record.FormKey, out var context)) {
            context = _editorEnvironment.ActiveModLinkCache.ResolveContext<TMajorRecord, TMajorRecordGetter>(record.FormKey);
        }

        var newOverride = context.GetOrAddAsOverride(_editorEnvironment.ActiveMod);
        if (context.ModKey != _editorEnvironment.ActiveMod.ModKey) {
            _logger.Here().Verbose("Creating overwrite of record {Record} in {Mod}",
                record, _editorEnvironment.ActiveMod.ModKey);

            _recordChanged.OnNext(newOverride);
        }

        return newOverride;
    }

    public IMajorRecord GetOrAddOverride(IMajorRecordGetter record) {
        if (!_editorEnvironment.LinkCache.TryResolveContext(record.FormKey, record.Registration.GetterType, out var context)) {
            context = _editorEnvironment.ActiveModLinkCache.ResolveContext(record);
        }

        var newOverride = context.GetOrAddAsOverride(_editorEnvironment.ActiveMod);
        if (context.ModKey != _editorEnvironment.ActiveMod.ModKey) {
            _logger.Here().Verbose("Creating overwrite of record {Record} in {Mod}",
                record, _editorEnvironment.ActiveMod.ModKey);

            _recordChanged.OnNext(newOverride);
        }

        return newOverride;
    }

    public void ReplaceReferences(IReferencedRecord record, IMajorRecordGetter replacingRecord) {
        var remap = new Dictionary<FormKey, FormKey> { { record.FormKey, replacingRecord.FormKey } };

        foreach (var reference in record.References.ToList()) {
            if (!_editorEnvironment.LinkCache.TryResolve(reference, out var referenceRecord)) continue;

            var overrideRecord = GetOrAddOverride(referenceRecord);
            RegisterUpdate(overrideRecord, () => overrideRecord.RemapLinks(remap));
        }
    }

    public void RegisterUpdate(IMajorRecordGetter record, Action updateAction) {
        var changeSubject = new Subject<IMajorRecordGetter>();

        // Notify pre update
        _recordChangedDiff.OnNext(new UpdateAction<IMajorRecordGetter>(record, updateAction));

        updateAction();

        // Notify post update
        changeSubject.OnNext(record);
        _recordChanged.OnNext(record);
    }
}
