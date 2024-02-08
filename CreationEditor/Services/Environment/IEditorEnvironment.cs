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
    /// Emits when the active mod changes
    /// </summary>
    IObservable<ModKey> ActiveModChanged { get; }
    /// <summary>
    /// Emits when the load order changes
    /// </summary>
    IObservable<List<ModKey>> LoadOrderChanged { get; }

    void Update(Func<IEditorEnvironmentUpdater, IEditorEnvironmentResult> applyUpdates);
}

public interface IEditorEnvironment<TMod, TModGetter> : IEditorEnvironment
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {
    public IGameEnvironment<TMod, TModGetter> Environment { get; protected set; }
    public new ILinkCache<TMod, TModGetter> LinkCache => Environment.LinkCache;

    public new TMod ActiveMod { get; protected set; }
    public new ILinkCache<TMod, TModGetter> ActiveModLinkCache { get; protected set; }
}
