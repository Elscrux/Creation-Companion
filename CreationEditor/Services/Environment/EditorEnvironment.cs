using System.Reactive.Subjects;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
namespace CreationEditor.Services.Environment;

public sealed class EditorEnvironment<TMod, TModGetter> : IEditorEnvironment<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {

    private readonly Func<IEditorEnvironmentUpdater> _getUpdater;
    private readonly ILogger _logger;
    private readonly Func<Type, object?>? _resolver;

    public IGameEnvironment<TMod, TModGetter> Environment { get; set; }
    IGameEnvironment IEditorEnvironment.GameEnvironment => Environment;

    public ILinkCache LinkCache => Environment.LinkCache;

    public TMod ActiveMod { get; set; }
    IMod IEditorEnvironment.ActiveMod => ActiveMod;

    public ILinkCache<TMod, TModGetter> ActiveModLinkCache { get; set; }
    ILinkCache IEditorEnvironment.ActiveModLinkCache => ActiveModLinkCache;

    private readonly ReplaySubject<ModKey> _activeModChanged = new(1);
    public IObservable<ModKey> ActiveModChanged => _activeModChanged;

    private readonly ReplaySubject<List<ModKey>> _loadOrderChanged = new(1);
    public IObservable<List<ModKey>> LoadOrderChanged => _loadOrderChanged;

    private readonly ReplaySubject<ILinkCache> _linkCacheChanged = new(1);
    public IObservable<ILinkCache> LinkCacheChanged => _linkCacheChanged;

    public EditorEnvironment(
        Func<IEditorEnvironmentUpdater> getUpdater,
        IGameReleaseContext gameReleaseContext,
        ILogger logger,
        Func<Type, object?>? resolver = null) {
        _getUpdater = getUpdater;
        _logger = logger;
        _resolver = resolver;

        ActiveMod = ModInstantiator<TMod>.Activator(ModKey.Null, gameReleaseContext.Release);
        ActiveModLinkCache = ActiveMod.ToMutableLinkCache<TMod, TModGetter>();

        var builder = GameEnvironmentBuilder<TMod, TModGetter>.Create(gameReleaseContext.Release);
        if (_resolver is not null) builder = builder.WithResolver(_resolver);
        Environment = builder
            .WithLoadOrder(ActiveMod.ModKey)
            .Build();
    }

    public void Update(Func<IEditorEnvironmentUpdater, IEditorEnvironmentResult> applyUpdates) {
        // Set up updater with current environment state
        var updater = _getUpdater();
        updater.SetTo(this);

        // Call updater
        if (applyUpdates(updater) is not EditorEnvironmentResult<TMod, TModGetter> result) {
            throw new InvalidOperationException("Editor Environment Updater did not return an EditorEnvironmentResult");
        }

        var modsString = string.Join(' ', updater.GetLoadOrder().Select(modKey => modKey.FileName));
        _logger.Here().Information("Loading mods {LoadedMods} with active mod {ActiveMod}", modsString, result.ActiveMod.ModKey.FileName);

        // Build new environment
        var builder = result.EnvironmentBuilder;
        if (_resolver is not null) {
            builder = builder.WithResolver(_resolver);
        }

        ActiveMod = result.ActiveMod;
        ActiveModLinkCache = ActiveMod.ToMutableLinkCache<TMod, TModGetter>();
        Environment = builder.Build();

        // Emit changes
        _activeModChanged.OnNext(ActiveMod.ModKey);
        _loadOrderChanged.OnNext(Environment.LinkCache.ListedOrder.Select(x => x.ModKey).ToList());
        _linkCacheChanged.OnNext(Environment.LinkCache);
    }
}
