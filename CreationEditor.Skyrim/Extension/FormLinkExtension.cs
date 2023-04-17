using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim; 

public static class FormLinkExtension {
    public static string? GetSelfOrBaseEditorID(this IFormLinkGetter<IPlacedGetter> formLink, ILinkCache linkCache) {
        var placed = formLink.TryResolve(linkCache);
        return placed?.GetSelfOrBaseEditorID(linkCache);
    }
}
