using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Environment;

public interface IEditorEnvironment : ILinkCacheProvider {
    /// <summary>
    /// Game environment for the editor
    /// </summary>
    IGameEnvironment GameEnvironment { get; }
    /// <summary>
    /// A list of all loaded mods in the current game environment
    /// </summary>
    IEnumerable<ModKey> LoadedMods => GameEnvironment.LoadOrder.Keys;

    /// <summary>
    /// Currently active mod
    /// </summary>
    IMod ActiveMod { get; }
    /// <summary>
    /// Link cache of the active mod
    /// </summary>
    ILinkCache ActiveModLinkCache { get; } //todo potentially remove

    /// <summary>
    /// Build the environment with the given mods and an active mod
    /// </summary>
    /// <param name="modKeys">Load order of the mod to include in the environment</param>
    /// <param name="activeMod">Active mod of the environment</param>
    void Build(IEnumerable<ModKey> modKeys, ModKey activeMod);
    /// <summary>
    /// Build the environment with the given mods and a new mod
    /// </summary>
    /// <param name="modKeys">Load order of the mod to include in the environment</param>
    /// <param name="newModName">Name of the new mod</param>
    /// <param name="modType">Type of the new mod</param>
    void Build(IEnumerable<ModKey> modKeys, string newModName, ModType modType);

    /// <summary>
    /// Add a mutable mod to the end of the load order before the active mod of an existing environment and builds it again
    /// <example>
    /// Resulting load order:
    /// <list type="bullet">
    /// <item><description>Game.esm</description></item>
    /// <item><description>OtherMod.esp</description></item>
    /// <item><description>NewModPassedInThisMethod.esp</description></item>
    /// <item><description>ActiveMod.esp</description></item>
    /// </list>
    /// </example>
    /// </summary>
    /// <param name="mod">Mod to add</param>
    void AddMutableMod(IMod mod);

    /// <summary>
    /// Remove a mutable mod added with <see cref="AddMutableMod"/> from the environment and builds it again
    /// </summary>
    /// <param name="mod">Mod to remove</param>
    void RemoveMutableMod(IMod mod);

    /// <summary>
    /// Emits when the active mod changes
    /// </summary>
    IObservable<ModKey> ActiveModChanged { get; }
    /// <summary>
    /// Emits when the load order changes
    /// </summary>
    IObservable<List<ModKey>> LoadOrderChanged { get; }
}

public interface IEditorEnvironment<TMod, TModGetter> : IEditorEnvironment
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {
    public IGameEnvironment<TMod, TModGetter> Environment { get; protected set; }
    public new ILinkCache<TMod, TModGetter> LinkCache => Environment.LinkCache;

    public new TMod ActiveMod { get; protected set; }
    public new ILinkCache<TMod, TModGetter> ActiveModLinkCache { get; protected set; }
}
