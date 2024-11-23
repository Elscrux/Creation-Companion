using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Record;

public interface IRecordController {
    #region CreateRecord
    /// <summary>
    /// Create a record of the given type in the active mod.
    /// </summary>
    /// <param name="type">Getter type of record</param>
    /// <returns>Created record</returns>
    IMajorRecord CreateRecord(System.Type type);

    /// <summary>
    /// Create a record of the given type in the given mod.
    /// </summary>
    /// <param name="type">Getter type of record</param>
    /// <param name="mod">Mod to get the override for</param>
    IMajorRecord CreateRecord(System.Type type, IMod mod);

    /// <summary>
    /// Create a record of type <typeparamref name="TMajorRecord"/> in the active mod.
    /// </summary>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    /// <returns>Created record</returns>
    TMajorRecord CreateRecord<TMajorRecord, TMajorRecordGetter>()
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter;

    /// <summary>
    /// Create a record of type <typeparamref name="TMajorRecord"/> in the given mod.
    /// </summary>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    /// <param name="mod">Mod to get the override for</param>
    /// <returns>Created record</returns>
    TMajorRecord CreateRecord<TMajorRecord, TMajorRecordGetter>(IMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter;
    #endregion

    #region DuplicateRecord
    /// <summary>
    /// Create a duplicate record the given record in the active mod.
    /// </summary>
    /// <param name="record">Record to duplicate</param>
    /// <returns>Duplicated record</returns>
    IMajorRecord DuplicateRecord(IMajorRecordGetter record);

    /// <summary>
    /// Create a duplicate record the given record in the given mod.
    /// </summary>
    /// <param name="record">Record to duplicate</param>
    /// <param name="mod">Mod to duplicate the record in</param>
    /// <returns>Duplicated record</returns>
    IMajorRecord DuplicateRecord(IMajorRecordGetter record, IMod mod);

    /// <summary>
    /// Create a duplicate record the given record in the active mod.
    /// </summary>
    /// <param name="record">Record to duplicate</param>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    /// <returns>Duplicated record</returns>
    TMajorRecord DuplicateRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter;

    /// <summary>
    /// Create a duplicate record the given record in the given mod.
    /// </summary>
    /// <param name="record">Record to duplicate</param>
    /// <param name="mod">Mod to duplicate the record in</param>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    /// <returns>Duplicated record</returns>
    TMajorRecord DuplicateRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record, IMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter;
    #endregion

    #region DeleteRecord
    /// <summary>
    /// Delete the given record in the active mod.
    /// If the record was defined in another mod, this will not delete it there, but delete the override in the active mod. 
    /// </summary>
    /// <param name="record">Record to delete</param>
    void DeleteRecord(IMajorRecordGetter record);

    /// <summary>
    /// Delete the given record in the given mod.
    /// If the record was defined in another mod, this will not delete it there, but delete the override in the active mod. 
    /// </summary>
    /// <param name="record">Record to delete</param>
    /// <param name="mod">Mod to delete the record from</param>
    void DeleteRecord(IMajorRecordGetter record, IMod mod);

    /// <summary>
    /// Delete the given record in the active mod.
    /// If the record was defined in another plugin, this will not delete it there, but delete the override in the active mod. 
    /// </summary>
    /// <param name="record">Record to delete</param>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    void DeleteRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter;

    /// <summary>
    /// Delete the given record in the given mod.
    /// If the record was defined in another plugin, this will not delete it there, but delete the override in the active mod. 
    /// </summary>
    /// <param name="record">Record to delete</param>
    /// <param name="mod">Mod to delete the record from</param>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    void DeleteRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record, IMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter;
    #endregion

    #region GetOrAddOverride
    /// <summary>
    /// Creates an override of an existing record in the active mod if it doesn't exist yet.
    /// </summary>
    /// <param name="record">Record to get an override for</param>
    /// <returns>Overwrite of the given record in the active mod</returns>
    IMajorRecord GetOrAddOverride(IMajorRecordGetter record);

    /// <summary>
    /// Creates an override of an existing record in the given mod if it doesn't exist yet.
    /// </summary>
    /// <param name="record">Record to get an override for</param>
    /// <param name="mod">Mod to get the override for</param>
    /// <returns>Overwrite of the given record in the active mod</returns>
    IMajorRecord GetOrAddOverride(IMajorRecordGetter record, IMod mod);

    /// <summary>
    /// Creates an override of an existing record in the active mod if it doesn't exist yet.
    /// </summary>
    /// <param name="record">Record to get an override for</param>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    /// <returns>Overwrite of the given record in the active mod</returns>
    TMajorRecord GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter;

    /// <summary>
    /// Creates an override of an existing record in the given mod if it doesn't exist yet.
    /// </summary>
    /// <param name="record">Record to get an override for</param>
    /// <param name="mod">Mod to get the override for</param>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    /// <returns>Overwrite of the given record in the active mod</returns>
    TMajorRecord GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record, IMod mod)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter;
    #endregion

    #region InjectRecords
    /// <summary>
    /// Injects the given records into the given mod.
    /// </summary>
    /// <param name="records">Records to inject</param>
    /// <param name="injectionTarget">Mod to inject the records into, this uses free form ids of the mod</param> 
    /// <param name="newRecordMod">Mod to create new, injected records in</param>
    /// <param name="editMod">Mod to replace existing records with injected records in</param>
    /// <param name="referenceGetter">Function to get references of a record</param>
    /// <param name="editorIdMapper">Function to map editor ids of the records</param>
    /// <param name="forceDelete">If true, the old record will be deleted even if it is referenced by other records (note that the known references will be updated, so this is just a setting for extra security)</param>
    /// <returns>Injected records in the same order as the input records</returns>
    IReadOnlyList<IMajorRecord> InjectRecords(
        IReadOnlyList<IMajorRecordGetter> records,
        IModGetter injectionTarget,
        IMod newRecordMod,
        IMod editMod,
        Func<FormKey, IEnumerable<IFormLinkIdentifier>> referenceGetter,
        Func<IMajorRecordGetter, string?> editorIdMapper,
        bool forceDelete = false);
    #endregion

    #region MarkForDeletion
    /// <summary>
    /// Marks a record for deletion in the given mod.
    /// </summary>
    /// <param name="record">Record to mark for deletion</param>
    /// <param name="mod">Mod to delete the record from</param>
    /// <param name="recordReferenceController">Record reference controller to update references</param>
    /// <param name="forceDelete">If true, the record will be deleted even if it is referenced by other records</param>
    /// <returns>True if the record was marked for deletion, false if it was not possible to delete the record</returns>
    bool MarkForDeletion(IMajorRecordGetter record, IMod mod, IRecordReferenceController recordReferenceController, bool forceDelete = false);

    /// <summary>
    /// Marks a record for deletion in the given mod.
    /// </summary>
    /// <param name="record">Record to mark for deletion</param>
    /// <param name="mod">Mod to delete the record from</param>
    /// <param name="referenceGetter">Function to get references of the record</param>
    /// <param name="forceDelete">If true, the record will be deleted even if it is referenced by other records</param>
    /// <returns>True if the record was marked for deletion, false if it was not possible to delete the record</returns>
    bool MarkForDeletion(IMajorRecordGetter record, IMod mod, Func<IEnumerable<IFormLinkIdentifier>> referenceGetter, bool forceDelete = false);
    #endregion

    #region ReplaceReferences
    /// <summary>
    /// Replaces all references of a record with with another record.
    /// </summary>
    /// <param name="record">Record to replace references for</param>
    /// <param name="replacingRecord">Record to replace references with</param>
    void ReplaceReferences(IReferencedRecord record, IMajorRecordGetter replacingRecord);

    /// <summary>
    /// Replaces given references of a record with with another record.
    /// </summary>
    /// <param name="record">Record to replace references for</param>
    /// <param name="references">References to record to edit</param>
    /// <param name="replacingRecord">Record to replace references with</param>
    void ReplaceReferences(IMajorRecordGetter record, IEnumerable<IFormLinkIdentifier> references, IMajorRecordGetter replacingRecord);
    #endregion

    #region RegisterUpdate
    /// <summary>
    /// Registers updates to a given record in the active mod. The update must be executed in the passed action.
    /// </summary>
    /// <param name="record">Record which updates are registered for</param>
    /// <param name="updateAction">An action that must update to the given record</param>
    void RegisterUpdate(IMajorRecord record, Action updateAction);

    /// <summary>
    /// Registers updates to a given record in the given mod. The update must be executed in the passed action.
    /// </summary>
    /// <param name="record">Record which updates are registered for</param>
    /// <param name="mod">Mod to register the update for</param>
    /// <param name="updateAction">An action that must update to the given record</param>
    void RegisterUpdate(IMajorRecord record, IMod mod, Action updateAction);
    #endregion

    /// <summary>
    /// Can be subscribed to in order to call a function before and after record changes
    /// </summary>
    JoinedObservable<RecordModPair> RecordChangedDiff { get; }

    /// <summary>
    /// Emits a record whenever an existing record was changed
    /// </summary>
    IObservable<IMajorRecordGetter> RecordInActiveModChanged { get; }
    IObservable<RecordModPair> RecordChanged { get; }
    IObservable<RecordModPair> WinningRecordChanged { get; }

    /// <summary>
    /// Emits a record whenever an existing record was created
    /// </summary>
    IObservable<IMajorRecordGetter> RecordInActiveModCreated { get; }
    IObservable<RecordModPair> RecordCreated { get; }

    /// <summary>
    /// Emits a record whenever an existing record was deleted
    /// </summary>
    IObservable<IMajorRecordGetter> RecordInActiveModDeleted { get; }
    IObservable<RecordModPair> RecordDeleted { get; }
    IObservable<RecordModPair> WinningRecordDeleted { get; }
}
