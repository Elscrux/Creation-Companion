using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Environment;

public static class EditorEnvironmentMixIn {
    public static void AddMutableMod(this IEditorEnvironment editorEnvironment, IMod mod) {
        editorEnvironment.Update(updater => updater.LoadOrder.AddMutableMod(mod).Build());
    }

    public static IMod AddNewMutableMod(this IEditorEnvironment editorEnvironment, ModKey modKey) {
        editorEnvironment.Update(updater => updater.LoadOrder.AddNewMutableMod(modKey).Build());
        return (IMod) editorEnvironment.GameEnvironment.LinkCache.ListedOrder.First(x => x.ModKey == modKey);
    }

    public static void RemoveMutableMod(this IEditorEnvironment editorEnvironment, IMod mod) {
        editorEnvironment.Update(updater => updater.LoadOrder.RemoveMutableMod(mod).Build());
    }

    public static void SetActive(this IEditorEnvironment editorEnvironment, ModKey modKey) {
        editorEnvironment.Update(updater => updater.ActiveMod.Existing(modKey).Build());
    }

    public static void SetActive(this IEditorEnvironment editorEnvironment, string newModName, ModType modType = ModType.Plugin) {
        editorEnvironment.Update(updater => updater.ActiveMod.New(newModName, modType).Build());
    }
}
