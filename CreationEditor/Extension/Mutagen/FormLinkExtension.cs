using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor;

public static class FormLinkExtension {
    public static string? GetEditorID<T>(this IFormLinkGetter<T> formLink, ILinkCache linkCache)
        where T : class, IMajorRecordGetter {
        return formLink.TryResolveIdentifier(linkCache, out var editorID) ? editorID : null;
    }
}
