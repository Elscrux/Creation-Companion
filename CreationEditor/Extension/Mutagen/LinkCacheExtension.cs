using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor;

public static class LinkCacheExtension {
    extension(ILinkCache linkCache) {
        public IModGetter GetMod(ModKey modKey) {
            return linkCache.ListedOrder.First(mod => mod.ModKey == modKey);
        }
        public IModGetter? ResolveMod(ModKey? modKey) {
            return modKey is null ? null : linkCache.ListedOrder.FirstOrDefault(mod => mod.ModKey == modKey);
        }
        public IEnumerable<IModGetter> ResolveMods(IEnumerable<ModKey> modKeys) {
            return modKeys.Select(modKey => linkCache.ResolveMod(modKey)).WhereNotNull();
        }
    }

    extension<TModSetter, TModGetter>(ILinkCache<TModSetter, TModGetter> linkCache)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        public TModGetter GetMod(ModKey modKey) {
            return (TModGetter) linkCache.ListedOrder.First(mod => mod.ModKey == modKey);
        }
        public TModGetter? ResolveMod(ModKey? modKey) {
            return modKey is null ? null : linkCache.ListedOrder.FirstOrDefault(mod => mod.ModKey == modKey) as TModGetter;
        }
        public IEnumerable<TModGetter> ResolveMods(IEnumerable<ModKey> modKeys) {
            return modKeys.Select(modKey => linkCache.ResolveMod(modKey)).WhereNotNull();
        }
    }
}
