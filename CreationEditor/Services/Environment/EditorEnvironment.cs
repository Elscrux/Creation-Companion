using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Environment;

public interface IEditorEnvironment<TMod, TModGetter> : IEditorEnvironment
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {
    public IGameEnvironment<TMod, TModGetter> Environment { get; protected set; }
    public new ILinkCache<TMod, TModGetter> LinkCache => Environment.LinkCache;
    
    public new TMod ActiveMod { get; protected set; }
    public new ILinkCache<TMod, TModGetter> ActiveModLinkCache { get; protected set; }
}
