using System.Reactive;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
namespace CreationEditor.Services.Filter;

public interface IModScopeProvider {
    BrowserScope Scope { get; set; }

    public ILinkCache LinkCache { get; }
    IEnumerable<ModKey> SelectedMods { get; }

    public IObservable<Unit> ScopeChanged { get; }
    public IObservable<ILinkCache> LinkCacheChanged { get; }
}
