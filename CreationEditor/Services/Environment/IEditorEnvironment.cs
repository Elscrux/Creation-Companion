using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Environment;

public interface IEditorEnvironment {
    public IGameEnvironment GameEnvironment { get; }
    public ILinkCache LinkCache => GameEnvironment.LinkCache;
    public IEnumerable<ModKey> LoadedMods => GameEnvironment.LoadOrder.Keys;

    public IMod ActiveMod { get; }
    public ILinkCache ActiveModLinkCache { get; } //todo potentially remove

    public void Build(IEnumerable<ModKey> modKeys, ModKey? activeMod = null);

    public IObservable<ModKey> ActiveModChanged { get; }
    public IObservable<List<ModKey>> LoadOrderChanged { get; }
    public IObservable<ILinkCache> LinkCacheChanged { get; }
}
