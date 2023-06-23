using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Environment;

public interface IEditorEnvironment {
    /// <summary>
    /// Game environment for the editor
    /// </summary>
    IGameEnvironment GameEnvironment { get; }
    /// <summary>
    /// Shortcut to the LinkCache of the game environment
    /// </summary>
    ILinkCache LinkCache => GameEnvironment.LinkCache;
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
    /// Emits when the active mod changes
    /// </summary>
    IObservable<ModKey> ActiveModChanged { get; }
    /// <summary>
    /// Emits when the load order changes
    /// </summary>
    IObservable<List<ModKey>> LoadOrderChanged { get; }
    /// <summary>
    /// Emits when the link cache changes
    /// </summary>
    IObservable<ILinkCache> LinkCacheChanged { get; }
}
