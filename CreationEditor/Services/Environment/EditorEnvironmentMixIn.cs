using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Environment;

public static class EditorEnvironmentMixIn {
    extension(IEditorEnvironment editorEnvironment) {
        public IMod AddNewMutableMod(ModKey modKey) {
            editorEnvironment.Update(updater => updater.LoadOrder.AddNewMutableMod(modKey).Build());
            return (IMod) editorEnvironment.GameEnvironment.LinkCache.ListedOrder.First(x => x.ModKey == modKey);
        }
        public TMod AddNewMutableMod<TMod>(ModKey modKey) {
            editorEnvironment.Update(updater => updater.LoadOrder.AddNewMutableMod(modKey).Build());
            return (TMod) editorEnvironment.GameEnvironment.LinkCache.ListedOrder.First(x => x.ModKey == modKey);
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
        public IModGetter GetMod(ModKey modKey) {
            return editorEnvironment.LinkCache.GetMod(modKey);
        }
        public IModGetter? ResolveMod(ModKey? modKey) {
            return editorEnvironment.LinkCache.ResolveMod(modKey);
        }
        public IEnumerable<IModGetter> ResolveMods(IEnumerable<ModKey> modKeys) {
            return editorEnvironment.LinkCache.ResolveMods(modKeys);
        }
    }

    extension<TModSetter, TModGetter>(IEditorEnvironment<TModSetter, TModGetter> editorEnvironment)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        public TModGetter GetMod(ModKey modKey) {
            return editorEnvironment.LinkCache.GetMod(modKey);
        }
        public TModGetter? ResolveMod(ModKey? modKey) {
            return editorEnvironment.LinkCache.ResolveMod(modKey);
        }
        public IEnumerable<TModGetter> ResolveMods(IEnumerable<ModKey> modKeys) {
            return editorEnvironment.LinkCache.ResolveMods(modKeys);
        }
    }

}
