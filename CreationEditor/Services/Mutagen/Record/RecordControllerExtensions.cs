using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Record;

public static class RecordControllerExtensions {
    extension(IRecordController recordController) {
        public TMajorRecord GetOrCreateRecord<TMajorRecord, TMajorRecordGetter>(string editorId)
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
            where TMajorRecordGetter : class, IMajorRecordGetter {
            if (recordController.LinkCacheProvider.LinkCache.TryResolve<TMajorRecordGetter>(editorId, out var existingRecord)) {
                return recordController.GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(existingRecord);
            }

            return recordController.CreateRecord<TMajorRecord, TMajorRecordGetter>();
        }
    }
}
