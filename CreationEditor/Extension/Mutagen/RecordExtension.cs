using CreationEditor.Services.Mutagen.References.Record.Controller;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor;

public static class RecordExtension {
    #region MarkForDeletion
    public static bool MarkForDeletion<TMod, TModGetter>(
        this IMajorRecordGetter record,
        ILinkCache<TMod, TModGetter> linkCache,
        TMod currentMod,
        IRecordReferenceController recordReferenceController,
        bool forceDelete = false)
        where TMod : class, TModGetter, IMod
        where TModGetter : class, IModGetter {
        return record.MarkForDeletion(linkCache, currentMod, () => recordReferenceController.GetReferences(record.FormKey), forceDelete);
    }

    public static bool MarkForDeletion<TMod, TModGetter>(
        this IMajorRecordGetter record,
        ILinkCache<TMod, TModGetter> linkCache,
        TMod currentMod,
        Func<IEnumerable<IFormLinkIdentifier>> referenceGetter,
        bool forceDelete = false)
        where TMod : class, TModGetter, IMod
        where TModGetter : class, IModGetter {
        if (!linkCache.TryResolveContext(record.ToLinkFromRuntimeType(), out var recordContext)) return false;

        var overrideRecord = recordContext.GetOrAddAsOverride(currentMod);

        return overrideRecord.MarkForDeletion(referenceGetter, forceDelete);
    }

    public static bool MarkForDeletion(
        this IMajorRecord record,
        IRecordReferenceController recordReferenceController,
        bool forceDelete = false) {
        return record.MarkForDeletion(() => recordReferenceController.GetReferences(record.FormKey), forceDelete);
    }

    public static bool MarkForDeletion(
        this IMajorRecord record,
        Func<IEnumerable<IFormLinkIdentifier>> referenceGetter,
        bool forceDelete = false) {
        if (forceDelete || !referenceGetter().Any()) {
            // No references available - we can delete this record
            record.IsDeleted = true;
        } else {
            // Otherwise mark record for deletion
            record.EditorID = "xDELETE" + record.EditorID;
        }

        return true;
    }
    #endregion
}
