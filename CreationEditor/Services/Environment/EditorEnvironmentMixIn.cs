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

    public static IMod GetMutableMod(this IEditorEnvironment editorEnvironment, ModKey modKey) {
        return editorEnvironment.MutableMods.First(x => x.ModKey == modKey);
    }

    public static TModGetter GetMod<TModSetter, TModGetter>(this IEditorEnvironment<TModSetter, TModGetter> editorEnvironment, ModKey modKey)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        return editorEnvironment.LinkCache.GetMod(modKey);
    }

    public static IModGetter GetMod(this IEditorEnvironment environment, ModKey modKey) {
        return environment.LinkCache.GetMod(modKey);
    }

    public static TModGetter? ResolveMod<TModSetter, TModGetter>(this IEditorEnvironment<TModSetter, TModGetter> editorEnvironment, ModKey? modKey)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        return editorEnvironment.LinkCache.ResolveMod(modKey);
    }

    public static IModGetter? ResolveMod(this IEditorEnvironment environment, ModKey? modKey) {
        return environment.LinkCache.ResolveMod(modKey);
    }

    public static IEnumerable<TModGetter> ResolveMods<TModSetter, TModGetter>(this IEditorEnvironment<TModSetter, TModGetter> editorEnvironment, IEnumerable<ModKey> modKeys)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        return editorEnvironment.LinkCache.ResolveMods(modKeys);
    }

    public static IEnumerable<IModGetter> ResolveMods(this IEditorEnvironment editorEnvironment, IEnumerable<ModKey> modKeys) {
        return editorEnvironment.LinkCache.ResolveMods(modKeys);
    }
}
