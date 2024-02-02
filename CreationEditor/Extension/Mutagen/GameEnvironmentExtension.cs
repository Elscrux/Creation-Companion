using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor;

public static class GameEnvironmentExtension {
    public static TModGetter GetMod<TModSetter, TModGetter>(this IGameEnvironment<TModSetter, TModGetter> environment, ModKey modKey)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        return environment.LoadOrder.GetMod(modKey);
    }

    public static IModGetter GetMod(this IGameEnvironment environment, ModKey modKey) {
        return environment.LoadOrder.GetMod(modKey);
    }

    public static TModGetter? ResolveMod<TModSetter, TModGetter>(this IGameEnvironment<TModSetter, TModGetter> environment, ModKey? modKey)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        return environment.LoadOrder.ResolveMod(modKey);
    }

    public static IModGetter? ResolveMod(this IGameEnvironment environment, ModKey? modKey) {
        return environment.LoadOrder.ResolveMod(modKey);
    }

    public static IEnumerable<TModGetter> ResolveMods<TModSetter, TModGetter>(this IGameEnvironment<TModSetter, TModGetter> environment, IEnumerable<ModKey> modKeys)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        return environment.LoadOrder.ResolveMods(modKeys);
    }

    public static IEnumerable<IModGetter> ResolveMods(this IGameEnvironment environment, IEnumerable<ModKey> modKeys) {
        return environment.LoadOrder.ResolveMods(modKeys);
    }
}
