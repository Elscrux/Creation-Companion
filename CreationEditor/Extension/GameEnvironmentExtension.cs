using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Extension;

public static class GameEnvironmentExtension {
    public static TModGetter? ResolveMod<TModSetter, TModGetter>(this IGameEnvironment<TModSetter, TModGetter> environment, ModKey? modKey)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        return modKey == null ? null : environment.LoadOrder.FirstOrDefault(mod => mod.Key == modKey)?.Value.Mod;
    }
    
    public static IModGetter? ResolveMod(this IGameEnvironment environment, ModKey? modKey) {
        return modKey == null ? null : environment.LoadOrder.FirstOrDefault(mod => mod.Key == modKey)?.Value.Mod;
    }
    
    public static IEnumerable<TModGetter> ResolveMods<TModSetter, TModGetter>(this IGameEnvironment<TModSetter, TModGetter> environment, IEnumerable<ModKey> modKeys)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter> {
        return modKeys.Select(modKey => environment.ResolveMod(modKey)).NotNull();
    }
    
    public static IEnumerable<IModGetter> ResolveMods(this IGameEnvironment environment, IEnumerable<ModKey> modKeys) {
        return modKeys.Select(modKey => environment.ResolveMod(modKey)).NotNull();
    }
}
