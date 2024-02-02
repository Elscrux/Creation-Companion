using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor;

public static class LoadOrderExtension {
    public static TModGetter GetMod<TModGetter>(this ILoadOrderGetter<IModListingGetter<TModGetter>> loadOrder, ModKey modKey)
        where TModGetter : class, IModGetter {
        return loadOrder.ListedOrder.First(mod => mod.ModKey == modKey).Mod!;
    }

    public static IModGetter GetMod(this ILoadOrderGetter<IModListingGetter<IModGetter>> loadOrder, ModKey modKey) {
        return loadOrder.ListedOrder.First(mod => mod.ModKey == modKey).Mod!;
    }

    public static TModGetter? ResolveMod<TModGetter>(this ILoadOrderGetter<IModListingGetter<TModGetter>> loadOrder, ModKey? modKey)
        where TModGetter : class, IModGetter {
        return modKey is null ? null : loadOrder.ListedOrder.FirstOrDefault(mod => mod.ModKey == modKey)?.Mod;
    }

    public static IEnumerable<TModGetter> ResolveMods<TModGetter>(this ILoadOrderGetter<IModListingGetter<TModGetter>> loadOrder, IEnumerable<ModKey> modKeys)
        where TModGetter : class, IModGetter {
        return modKeys.Select(modKey => loadOrder.ResolveMod(modKey)).NotNull();
    }

    public static IEnumerable<IModGetter> ResolveMods(this ILoadOrderGetter<IModListingGetter<IModGetter>> loadOrder, IEnumerable<ModKey> modKeys) {
        return modKeys.Select(modKey => loadOrder.ResolveMod(modKey)).NotNull();
    }
}
