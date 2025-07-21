using System.Reactive.Linq;
using System.Reactive.Subjects;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
namespace CreationEditor.Services.Mutagen.Record;

public sealed class RecordController<TMod, TModGetter> : IRecordController
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {
    private readonly ILogger _logger;
    private readonly IMutagenTypeProvider _mutagenTypeProvider;
    private readonly IEditorEnvironment<TMod, TModGetter> _editorEnvironment;

    private IObservable<IMajorRecordGetter> OnlyActiveMod(IObservable<RecordModPair> observable) {
        return observable
            .Where(x => x.Mod.ModKey == _editorEnvironment.ActiveMod.ModKey)
            .Select(x => x.Item1);
    }

    private IObservable<RecordModPair> OnlyWinningRecord(IObservable<RecordModPair> observable) {
        return observable.Where(x => {
            // The active mod is loaded last, so it will always be the winning override
            if (x.Mod.ModKey == _editorEnvironment.ActiveMod.ModKey) return true;

            // If the active mod is not loaded, we need to check if the override is the winning override
            var context = _editorEnvironment.LinkCache.ResolveContext(x.Record.FormKey, x.Record.Registration.GetterType);
            return context.ModKey == x.Mod.ModKey;
        });
    }

    private readonly Subject<RecordModPair> _recordChanged = new();
    public IObservable<IMajorRecordGetter> RecordInActiveModChanged => OnlyActiveMod(_recordChanged);
    public IObservable<RecordModPair> RecordChanged => _recordChanged;
    public IObservable<RecordModPair> WinningRecordChanged => OnlyWinningRecord(_recordChanged);

    private readonly Subject<UpdateAction<RecordModPair>> _recordChangedDiff = new();
    public JoinedObservable<RecordModPair> RecordChangedDiff { get; }

    private readonly Subject<RecordModPair> _recordCreated = new();
    public IObservable<IMajorRecordGetter> RecordInActiveModCreated => OnlyActiveMod(_recordCreated);
    public IObservable<RecordModPair> RecordCreated => _recordCreated;

    private readonly Subject<RecordModPair> _recordDeleted = new();
    public IObservable<IMajorRecordGetter> RecordInActiveModDeleted => OnlyActiveMod(_recordDeleted);
    public IObservable<RecordModPair> RecordDeleted => _recordDeleted;
    public IObservable<RecordModPair> WinningRecordDeleted => OnlyWinningRecord(_recordDeleted);

    public RecordController(
        ILogger logger,
        IMutagenTypeProvider mutagenTypeProvider,
        IEditorEnvironment<TMod, TModGetter> editorEnvironment) {
        _logger = logger;
        _mutagenTypeProvider = mutagenTypeProvider;
        _editorEnvironment = editorEnvironment;
        RecordChangedDiff = new JoinedObservable<RecordModPair>(_recordChangedDiff);

        // Set up the record changed observable
        RecordChangedDiff.Subscribe(_ => pair => _recordChanged.OnNext(pair));
    }

    private static TTarget CastOrThrow<TTarget>(IModGetter mod) {
        if (mod is TTarget t) return t;

        throw new ArgumentException("Mod is not of the correct type", nameof(mod));
    }

    #region CreateRecord
    public IMajorRecord CreateRecord(System.Type type) => CreateRecord(type, _editorEnvironment.ActiveMod);
    public IMajorRecord CreateRecord(System.Type type, IMod mod) => CreateRecord(type, CastOrThrow<TMod>(mod));
    public IMajorRecord CreateRecord(System.Type type, TMod mod) {
        var group = mod.GetTopLevelGroup(type);
        var record = group.AddNew(mod.GetNextFormKey());

        _logger.Here().Verbose(
            "Creating new record {Record} of type {Type} in {Mod}",
            record,
            type,
            mod.ModKey);

        _recordCreated.OnNext((record, mod));

        return record;
    }

    public TMajorRecord CreateRecord<TMajorRecord, TMajorRecordGetter>()
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter => CreateRecord<TMajorRecord, TMajorRecordGetter>(_editorEnvironment.ActiveMod);
    public TMajorRecord CreateRecord<TMajorRecord, TMajorRecordGetter>(IMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter => CreateRecord<TMajorRecord, TMajorRecordGetter>(CastOrThrow<TMod>(mod));
    public TMajorRecord CreateRecord<TMajorRecord, TMajorRecordGetter>(TMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter {
        var group = mod.GetTopLevelGroup<TMajorRecord>();
        var record = group.AddNew(mod.GetNextFormKey());

        _logger.Here().Verbose("Creating new record {Record} of type {Type} in {Mod}", record, typeof(TMajorRecord), mod.ModKey);

        _recordCreated.OnNext((record, mod));

        return record;
    }

    public IMajorRecord CreateRecord(IMajorRecord record) => CreateRecord(record, _editorEnvironment.ActiveMod);
    public IMajorRecord CreateRecord(IMajorRecord record, IMod mod) {
        if (record.FormKey.ModKey != mod.ModKey) throw new ArgumentException("Record is not from the same mod", nameof(record));

        var group = mod.GetTopLevelGroup(record.Registration.GetterType);
        group.AddUntyped(record);
        
        _logger.Here().Verbose("Adding new record {Record} in {Mod}", record, mod.ModKey);

        _recordCreated.OnNext((record, mod));
        
        return record;
    }
    #endregion

    #region DuplicateRecord
    public IMajorRecord DuplicateRecord(IMajorRecordGetter record, IMod mod) => DuplicateRecord(record, CastOrThrow<TMod>(mod));
    public IMajorRecord DuplicateRecord(IMajorRecordGetter record) => DuplicateRecord(record, _editorEnvironment.ActiveMod);
    public IMajorRecord DuplicateRecord(IMajorRecordGetter record, TMod mod) {
        var resolveContext = _editorEnvironment.LinkCache.ResolveContext(record.FormKey, record.Registration.GetterType);
        var duplicate = resolveContext.DuplicateIntoAsNewRecord(mod);

        _logger.Here().Verbose("Creating new record {Duplicate} by duplicating {Record} in {Mod}", duplicate, record, mod.ModKey);

        _recordCreated.OnNext((duplicate, mod));

        return duplicate;
    }

    public TMajorRecord DuplicateRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter {
        return DuplicateRecord<TMajorRecord, TMajorRecordGetter>(record.ToStandardizedIdentifier(), _editorEnvironment.ActiveMod);
    }

    public TMajorRecord DuplicateRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record, IMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter {
        return DuplicateRecord<TMajorRecord, TMajorRecordGetter>(record.ToStandardizedIdentifier(), CastOrThrow<TMod>(mod));
    }

    public TMajorRecord DuplicateRecord<TMajorRecord, TMajorRecordGetter>(IFormLinkIdentifier record, TMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter {
        var resolveContext = _editorEnvironment.LinkCache.ResolveContext<TMajorRecord, TMajorRecordGetter>(record.FormKey);
        var duplicate = resolveContext.DuplicateIntoAsNewRecord(mod);

        _logger.Here().Verbose("Creating new record {Duplicate} by duplicating {Record} in {Mod}", duplicate, record, mod.ModKey);

        _recordCreated.OnNext((duplicate, mod));

        return duplicate;
    }
    #endregion

    #region DeleteRecord
    public void DeleteRecord(IMajorRecordGetter record) => DeleteRecord(record, _editorEnvironment.ActiveMod);
    public void DeleteRecord(IMajorRecordGetter record, IMod mod) => DeleteRecord(record, CastOrThrow<TMod>(mod));
    public void DeleteRecord(IMajorRecordGetter record, TMod mod) {
        var newOverride = GetOrAddOverride(record, mod);
        newOverride.IsDeleted = true;

        _logger.Here().Verbose("Deleting record {Record} from {Mod}", record, mod);

        _recordDeleted.OnNext((newOverride, mod));
    }

    public void DeleteRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter => DeleteRecord<TMajorRecord, TMajorRecordGetter>(record.ToStandardizedIdentifier(), _editorEnvironment.ActiveMod);
    public void DeleteRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record, IMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter => DeleteRecord<TMajorRecord, TMajorRecordGetter>(record.ToStandardizedIdentifier(), CastOrThrow<TMod>(mod));
    public void DeleteRecord<TMajorRecord, TMajorRecordGetter>(IFormLinkIdentifier record, TMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter {
        var newOverride = GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(record, mod);
        newOverride.IsDeleted = true;

        _logger.Here().Verbose("Deleting record {Record} from {Mod}", record, mod);

        _recordDeleted.OnNext((newOverride, mod));
    }
    #endregion

    #region GetOrAddOverride
    public IMajorRecord GetOrAddOverride(IFormLinkIdentifier record) => GetOrAddOverride(record, _editorEnvironment.ActiveMod);
    public IMajorRecord GetOrAddOverride(IFormLinkIdentifier record, IMod mod) => GetOrAddOverride(record, CastOrThrow<TMod>(mod));
    public IMajorRecord GetOrAddOverride(IFormLinkIdentifier record, TMod mod) {
        var context = _editorEnvironment.LinkCache.ResolveContext(record.FormKey, _mutagenTypeProvider.GetRecordGetterType(record.Type));

        var newOverride = context.GetOrAddAsOverride(mod);
        if (context.ModKey != mod.ModKey) {
            _logger.Here().Verbose("Creating overwrite of record {Record} in {Mod}", record, mod.ModKey);

            _recordChanged.OnNext((newOverride, mod));
        }

        return newOverride;
    }
    public IMajorRecord GetOrAddOverride(IMajorRecordGetter record) => GetOrAddOverride(record, _editorEnvironment.ActiveMod);
    public IMajorRecord GetOrAddOverride(IMajorRecordGetter record, IMod mod) => GetOrAddOverride(record, CastOrThrow<TMod>(mod));
    public IMajorRecord GetOrAddOverride(IMajorRecordGetter record, TMod mod) {
        var context = _editorEnvironment.LinkCache.ResolveContext(record.FormKey, record.Registration.GetterType);

        var newOverride = context.GetOrAddAsOverride(mod);
        if (context.ModKey != mod.ModKey) {
            _logger.Here().Verbose("Creating overwrite of record {Record} in {Mod}", record, mod.ModKey);

            _recordChanged.OnNext((newOverride, mod));
        }

        return newOverride;
    }

    public TMajorRecord GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter {
        return GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(record.ToStandardizedIdentifier(), _editorEnvironment.ActiveMod);
    }

    public TMajorRecord GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record, IMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter {
        return GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(record.ToStandardizedIdentifier(), CastOrThrow<TMod>(mod));
    }

    public TMajorRecord GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(IFormLinkIdentifier record, TMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter {
        var context = _editorEnvironment.LinkCache.ResolveContext<TMajorRecord, TMajorRecordGetter>(record.FormKey);

        var newOverride = context.GetOrAddAsOverride(mod);
        if (context.ModKey != mod.ModKey) {
            _logger.Here().Verbose("Creating overwrite of record {Record} in {Mod}", record, mod.ModKey);

            _recordChanged.OnNext((newOverride, mod));
        }

        return newOverride;
    }
    #endregion

    #region InjectRecords
    public IReadOnlyList<IMajorRecord> InjectRecords(
        IReadOnlyList<IMajorRecordGetter> records,
        IModGetter injectionTarget,
        IMod newRecordMod,
        IMod editMod,
        Func<IFormLinkIdentifier, IEnumerable<IFormLinkIdentifier>> referenceGetter,
        Func<IMajorRecordGetter, string?> editorIdMapper,
        bool forceDelete = false) {
        return InjectRecords(
            records,
            CastOrThrow<TModGetter>(injectionTarget),
            CastOrThrow<TMod>(newRecordMod),
            CastOrThrow<TMod>(editMod),
            referenceGetter,
            editorIdMapper,
            forceDelete);
    }

    public IReadOnlyList<IMajorRecord> InjectRecords(
        IReadOnlyList<IMajorRecordGetter> records,
        TModGetter injectionTarget,
        TMod newRecordMod,
        TMod editMod,
        Func<IFormLinkIdentifier, IEnumerable<IFormLinkIdentifier>> referenceGetter,
        Func<IMajorRecordGetter, string?> editorIdMapper,
        bool forceDelete = false) {

        // Find free IDs in the injection target mod
        var range = (int) Math.Pow(16, 6);
        var bitmap = uint.MaxValue - (uint) range;
        var freeIds = Enumerable.Range(0, range).Select(i => (uint) i).ToHashSet();
        foreach (var record in injectionTarget.EnumerateMajorRecords()) {
            freeIds.Remove(record.FormKey.ID & bitmap);
        }

        // Inject records into newRecordMod
        var remapData = new Dictionary<FormKey, FormKey>();
        var newRecords = new List<IMajorRecord>();
        foreach (var record in records) {
            // Select a random free ID for the new record
            var newId = freeIds.ElementAt(Random.Shared.Next(freeIds.Count));
            freeIds.Remove(newId);

            var targetFormKey = new FormKey(injectionTarget.ModKey, newId);
            var duplicate = record.Duplicate(targetFormKey);

            if (duplicate.EditorID is not null) duplicate.EditorID = editorIdMapper(duplicate);
            newRecordMod.GetTopLevelGroup((duplicate as IMajorRecordGetter).Registration.GetterType).AddUntyped(duplicate);
            _recordCreated.OnNext((duplicate, newRecordMod));

            remapData.Add(record.FormKey, targetFormKey);
            newRecords.Add(duplicate);
        }

        // Force remap references in the new records
        foreach (var newRecord in newRecords) {
            RegisterUpdate(newRecord, editMod, () => newRecord.RemapLinks(remapData));
        }

        // Remap references to use the new injected records
        foreach (var record in records) {
            var references = referenceGetter(record).ToArray();

            // Remap references to use the new injected records
            foreach (var reference in references) {
                if (!_editorEnvironment.LinkCache.TryResolveContext(reference, out var context)) continue;

                var overrideReference = GetOrAddOverride(context.Record, editMod);
                RegisterUpdate(overrideReference, editMod, () => overrideReference.RemapLinks(remapData));
            }

            // Mark original records for deletion
            MarkForDeletion(record, editMod, () => references, forceDelete);
        }

        return newRecords;
    }
    #endregion

    #region MarkForDeletion
    public bool MarkForDeletion(
        IMajorRecordGetter record,
        IMod mod,
        IReferenceService referenceService,
        bool forceDelete = false) {
        return MarkForDeletion(record, CastOrThrow<TMod>(mod), () => referenceService.GetRecordReferences(record), forceDelete);
    }
    public bool MarkForDeletion(
        IMajorRecordGetter record,
        TMod mod,
        IReferenceService referenceService,
        bool forceDelete = false) {
        return MarkForDeletion(record, mod, () => referenceService.GetRecordReferences(record), forceDelete);
    }

    public bool MarkForDeletion(
        IMajorRecordGetter record,
        IMod mod,
        Func<IEnumerable<IFormLinkIdentifier>> referenceGetter,
        bool forceDelete = false) {
        return MarkForDeletion(record, CastOrThrow<TMod>(mod), referenceGetter, forceDelete);
    }
    public bool MarkForDeletion(
        IMajorRecordGetter record,
        TMod mod,
        Func<IEnumerable<IFormLinkIdentifier>> referenceGetter,
        bool forceDelete = false) {
        if (record is not IMajorRecord setter) {
            if (!_editorEnvironment.LinkCache.TryResolveContext(record.ToLinkFromRuntimeType(), out var recordContext)) return false;

            setter = GetOrAddOverride(recordContext.Record, mod);
        }

        if (forceDelete || !referenceGetter().Any()) {
            // No references available - we can delete this record
            DeleteRecord(setter);
        } else {
            // Otherwise mark record for deletion
            RegisterUpdate(setter, () => setter.EditorID = "xDELETE" + setter.EditorID);
        }

        return true;
    }
    #endregion

    #region RemapReferences
    public void RemapReferences(IReferencedRecord record, IMajorRecordGetter remappingRecord) {
        RemapReferences(record.Record, record.RecordReferences, remappingRecord);
    }
    public void RemapReferences(IMajorRecordGetter record, IEnumerable<IFormLinkIdentifier> references, IMajorRecordGetter remappingRecord) {
        RemapReferences(record, references, remappingRecord, _editorEnvironment.ActiveMod);
    }
    public void RemapReferences(
        IMajorRecordGetter record,
        IEnumerable<IFormLinkIdentifier> references,
        IMajorRecordGetter replacingRecord,
        TMod mod) {
        var remap = new Dictionary<FormKey, FormKey> { { record.FormKey, replacingRecord.FormKey } };

        foreach (var reference in references.ToArray()) {
            if (!_editorEnvironment.LinkCache.TryResolve(reference, out var referenceRecord)) continue;

            var overrideRecord = GetOrAddOverride(referenceRecord, mod);
            RegisterUpdate(overrideRecord, mod, () => overrideRecord.RemapLinks(remap));
        }
    }
    #endregion

    #region RegisterUpdate
    public void RegisterUpdate(IMajorRecord record, Action updateAction) => RegisterUpdate(record, _editorEnvironment.ActiveMod, updateAction);
    public void RegisterUpdate(IMajorRecord record, IMod mod, Action updateAction) {
        if (mod is TMod modT) {
            RegisterUpdate(record, modT, updateAction);
        } else {
            throw new ArgumentException("Mod is not of the correct type", nameof(mod));
        }
    }
    public void RegisterUpdate(IMajorRecord record, TMod mod, Action updateAction) {
        // Notify pre update
        var pair = (record, mod);
        _recordChangedDiff.OnNext(new UpdateAction<RecordModPair>(pair, updateAction));
    }
    #endregion
}
