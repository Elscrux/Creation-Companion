using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor;

public static class LinkCacheExtension {
    public static IModGetter GetMod(this ILinkCache linkCache, ModKey modKey) {
        return linkCache.ListedOrder.First(mod => mod.ModKey == modKey);
    }

    public static TModGetter GetMod<TModSetter, TModGetter>(this ILinkCache<TModSetter, TModGetter> linkCache, ModKey modKey)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        return (TModGetter) linkCache.ListedOrder.First(mod => mod.ModKey == modKey);
    }

    public static IModGetter? ResolveMod(this ILinkCache linkCache, ModKey? modKey) {
        return modKey is null ? null : linkCache.ListedOrder.FirstOrDefault(mod => mod.ModKey == modKey);
    }

    public static TModGetter? ResolveMod<TModSetter, TModGetter>(this ILinkCache<TModSetter, TModGetter> linkCache, ModKey? modKey)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        return modKey is null ? null : linkCache.ListedOrder.FirstOrDefault(mod => mod.ModKey == modKey) as TModGetter;
    }

    public static IEnumerable<IModGetter> ResolveMods(this ILinkCache linkCache, IEnumerable<ModKey> modKeys) {
        return modKeys.Select(modKey => linkCache.ResolveMod(modKey)).NotNull();
    }

    public static IEnumerable<TModGetter> ResolveMods<TModSetter, TModGetter>(this ILinkCache<TModSetter, TModGetter> linkCache, IEnumerable<ModKey> modKeys)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        return modKeys.Select(modKey => linkCache.ResolveMod(modKey)).NotNull();
    }
}
