using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor;

public static class FormLinkExtension {
    extension<T>(IFormLinkGetter<T> formLink)
        where T : class, IMajorRecordGetter {
        public string? GetEditorID(ILinkCache linkCache) {
            return formLink.TryResolveIdentifier(linkCache, out var editorID) ? editorID : null;
        }
    }
}
