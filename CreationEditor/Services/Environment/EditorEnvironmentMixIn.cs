using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Environment;

public static class EditorEnvironmentMixIn {
    extension(IEditorEnvironment editorEnvironment) {
        public IMod AddNewMutableMod(ModKey modKey) {
            editorEnvironment.Update(updater => updater.LoadOrder.AddNewMutableMod(modKey).Build());
            return editorEnvironment.GameEnvironment.LinkCache.ListedOrder.First(x => x.ModKey == modKey) as IMod
             ?? throw new InvalidOperationException($"Mod {modKey} is a read-only mod that is already present in the load order.");
        }
        public TMod AddNewMutableMod<TMod>(ModKey modKey) where TMod : class {
            editorEnvironment.Update(updater => updater.LoadOrder.AddNewMutableMod(modKey).Build());
            return editorEnvironment.GameEnvironment.LinkCache.ListedOrder.First(x => x.ModKey == modKey) as TMod
             ?? throw new InvalidOperationException($"Mod {modKey} is a read-only mod that is already present in the load order.");
        }
        public void RemoveMutableMod(IMod mod) {
            editorEnvironment.Update(updater => updater.LoadOrder.RemoveMutableMod(mod).Build());
        }
        public void SetActive(ModKey modKey) {
            editorEnvironment.Update(updater => updater.ActiveMod.Existing(modKey).Build());
        }
        public void SetActive(string newModName, ModType modType = ModType.Plugin) {
            editorEnvironment.Update(updater => updater.ActiveMod.New(newModName, modType).Build());
        }
        public IMod GetMutableMod(ModKey modKey) {
            return editorEnvironment.MutableMods.First(x => x.ModKey == modKey);
        }
        public IModGetter GetMod(ModKey modKey) => editorEnvironment.LinkCache.GetMod(modKey);
        public IModGetter? ResolveMod(ModKey? modKey) => editorEnvironment.LinkCache.ResolveMod(modKey);
        public IEnumerable<IModGetter> ResolveMods(IEnumerable<ModKey> modKeys) => editorEnvironment.LinkCache.ResolveMods(modKeys);
    }

    extension<TModSetter, TModGetter>(IEditorEnvironment<TModSetter, TModGetter> editorEnvironment)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        public TModGetter GetMod(ModKey modKey) => editorEnvironment.LinkCache.GetMod(modKey);
        public TModGetter? ResolveMod(ModKey? modKey) => editorEnvironment.LinkCache.ResolveMod(modKey);
        public IEnumerable<TModGetter> ResolveMods(IEnumerable<ModKey> modKeys) => editorEnvironment.LinkCache.ResolveMods(modKeys);
    }
}
