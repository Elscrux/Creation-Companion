using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor;

public static class MajorRecordQueryableGetterExtension {
    public static string? GetName(this IMajorRecordQueryableGetter recordQueryable) {
        return recordQueryable switch {
            INamedGetter { Name: {} name } => name,
            IMajorRecordGetter { EditorID: {} editorID } => editorID,
            IFormKeyGetter formKeyGetter => formKeyGetter.FormKey.ToString(),
            _ => recordQueryable.ToString(),
        };
    }
}
