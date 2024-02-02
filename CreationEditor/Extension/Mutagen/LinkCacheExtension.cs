using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor;

public static class LinkCacheExtension {
    public static IModGetter GetMod(this ILinkCache linkCache, ModKey modKey) {
        return linkCache.ListedOrder.First(mod => mod.ModKey == modKey);
    }

    public static IModGetter? ResolveMod(this ILinkCache linkCache, ModKey? modKey) {
        return modKey is null ? null : linkCache.ListedOrder.FirstOrDefault(mod => mod.ModKey == modKey);
    }

    public static IEnumerable<IModGetter> ResolveMods(this ILinkCache linkCache, IEnumerable<ModKey> modKeys) {
        return modKeys.Select(modKey => linkCache.ResolveMod(modKey)).NotNull();
    }
}
