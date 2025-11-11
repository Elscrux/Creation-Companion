using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor;

public static class GameEnvironmentExtension {
    extension<TModSetter, TModGetter>(IGameEnvironment<TModSetter, TModGetter> environment)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        public TModGetter GetMod(ModKey modKey) {
            return environment.LinkCache.GetMod(modKey);
        }
        public TModGetter? ResolveMod(ModKey? modKey) {
            return environment.LinkCache.ResolveMod(modKey);
        }
        public IEnumerable<TModGetter> ResolveMods(IEnumerable<ModKey> modKeys) {
            return environment.LinkCache.ResolveMods(modKeys);
        }
    }

    extension(IGameEnvironment environment) {
        public IModGetter GetMod(ModKey modKey) {
            return environment.LinkCache.GetMod(modKey);
        }
        public IModGetter? ResolveMod(ModKey? modKey) {
            return environment.LinkCache.ResolveMod(modKey);
        }
        public IEnumerable<IModGetter> ResolveMods(IEnumerable<ModKey> modKeys) {
            return environment.LinkCache.ResolveMods(modKeys);
        }
    }
}
