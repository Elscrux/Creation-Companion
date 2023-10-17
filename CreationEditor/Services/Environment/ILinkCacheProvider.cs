using Mutagen.Bethesda.Plugins.Cache;
namespace CreationEditor.Services.Environment;

public interface ILinkCacheProvider {
    /// <summary>
    /// Link Cache for retrieving records
    /// </summary>
    ILinkCache LinkCache { get; }

    /// <summary>
    /// Emits when the link cache changes
    /// </summary>
    IObservable<ILinkCache> LinkCacheChanged { get; }
}
