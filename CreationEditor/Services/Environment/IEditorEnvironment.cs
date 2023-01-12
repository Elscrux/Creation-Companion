using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Environment;

public interface IEditorEnvironment {
    public IGameEnvironment Environment { get; }
    public ILinkCache LinkCache => Environment.LinkCache;
    public IEnumerable<ModKey> LoadedMods => Environment.LoadOrder.Keys;
    
    public IMod ActiveMod { get; }
    public ILinkCache ActiveModLinkCache { get; } //todo potentially remove
    
    public void Build(IEnumerable<ModKey> modKeys, ModKey? activeMod = null);
    
    public IObservable<ModKey> ActiveModChanged { get; }
    public IObservable<List<ModKey>> LoadOrderChanged { get; }
}
