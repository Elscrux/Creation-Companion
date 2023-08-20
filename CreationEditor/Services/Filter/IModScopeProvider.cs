using System.Reactive;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Filter;

/// <summary>
/// Used to define a subset scope of mods in the current editor environment.
/// </summary>
public interface IModScopeProvider {
    /// <summary>
    /// Scope of the mods to search in
    /// </summary>
    BrowserScope Scope { get; set; }

    /// <summary>
    /// Link cache to use for the scope 
    /// </summary>
    ILinkCache LinkCache { get; }

    /// <summary>
    /// Selected mod keys in the scope
    /// </summary>
    IEnumerable<ModKey> SelectedModKeys { get; }

    /// <summary>
    /// Selected mods in the scope
    /// </summary>
    IEnumerable<IModGetter> SelectedMods { get; }

    /// <summary>
    /// Observable that fires when the scope changes
    /// </summary>
    IObservable<Unit> ScopeChanged { get; }

    /// <summary>
    /// Observable that fires when the link cache changes
    /// </summary>
    IObservable<ILinkCache> LinkCacheChanged { get; }
}
