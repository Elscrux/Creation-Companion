using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class FormLinkExtension {
    extension(IFormLinkGetter<IPlacedGetter> formLink) {
        public string? GetSelfOrBaseEditorID(ILinkCache linkCache) {
            var placed = formLink.TryResolve(linkCache);
            return placed?.GetSelfOrBaseEditorID(linkCache);
        }
    }
}
