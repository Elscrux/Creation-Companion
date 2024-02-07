using System.IO.Abstractions;
using System.Reactive.Subjects;
using CreationEditor.Services.Notification;
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

    private readonly IFileSystem _fileSystem;
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly INotificationService _notificationService;
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
        IFileSystem fileSystem,
        IGameReleaseContext gameReleaseContext,
        IDataDirectoryProvider dataDirectoryProvider,
        INotificationService notificationService,
        ILogger logger,
        Func<Type, object?>? resolver = null) {
        _fileSystem = fileSystem;
        _gameReleaseContext = gameReleaseContext;
        _dataDirectoryProvider = dataDirectoryProvider;
        _notificationService = notificationService;
        _logger = logger;
        _resolver = resolver;

        ActiveMod = ModInstantiator<TMod>.Activator(ModKey.Null, _gameReleaseContext.Release);
        ActiveModLinkCache = ActiveMod.ToMutableLinkCache<TMod, TModGetter>();

        var builder = GameEnvironmentBuilder<TMod, TModGetter>.Create(_gameReleaseContext.Release);
        if (_resolver is not null) builder = builder.WithResolver(_resolver);
        Environment = builder
            .WithLoadOrder(ActiveMod.ModKey)
            .Build();
    }

    public void Build(IEnumerable<ModKey> modKeys, ModKey activeMod) {
        var modKeysArray = modKeys.ToArray();

        var modsString = string.Join(' ', modKeysArray.Select(modKey => modKey.FileName));
        _logger.Here().Information("Loading mods {LoadedMods} with active mod {ActiveMod}", modsString, activeMod.FileName);

        var linearNotifier = new LinearNotifier(_notificationService, 2);

        linearNotifier.Next($"Preparing {activeMod.FileName}");
        var activeModPath = new ModPath(_fileSystem.Path.Combine(_dataDirectoryProvider.Path, activeMod.FileName));
        ActiveMod = ModInstantiator<TMod>.Importer(activeModPath, _gameReleaseContext.Release, _fileSystem);
        ActiveModLinkCache = ActiveMod.ToMutableLinkCache<TMod, TModGetter>();

        linearNotifier.Next("Building GameEnvironment");

        BuildEnvironment(modKeysArray, []);

        linearNotifier.Stop();
    }

    public void Build(IEnumerable<ModKey> modKeys, string newModName, ModType modType) {
        var modKeysArray = modKeys.ToArray();

        var modsString = string.Join(' ', modKeysArray.Select(modKey => modKey.FileName));
        _logger.Here().Information("Loading mods {LoadedMods} without active mod", modsString);

        ActiveMod = ModInstantiator<TMod>.Activator(new ModKey(newModName, modType), _gameReleaseContext.Release);
        ActiveModLinkCache = ActiveMod.ToMutableLinkCache<TMod, TModGetter>();

        var linearNotifier = new LinearNotifier(_notificationService);
        linearNotifier.Next("Building GameEnvironment");

        BuildEnvironment(modKeysArray, []);

        linearNotifier.Stop();
    }

    public void AddMutableMod(IMod mod) {
        if (mod is not TMod gameMod) {
            throw new ArgumentException($"Mod is not a {_gameReleaseContext.Release} mod", nameof(mod));
        }

        AddMutableMod(gameMod);
    }

    public void AddMutableMod(TMod mod) {
        // Add the mod to the load order but keep the active mod at the end
        var loadOrder = Environment.LinkCache.ListedOrder
            .Where(m => m.ModKey != ActiveMod.ModKey)
            .Select(m => m.ModKey)
            .Append(mod.ModKey)
            .Append(ActiveMod.ModKey)
            .Distinct()
            .ToArray();

        BuildEnvironment(loadOrder, [mod]);
    }

    public void RemoveMutableMod(IMod mod) {
        if (mod is not TMod gameMod) {
            throw new ArgumentException($"Mod is not a {_gameReleaseContext.Release} mod", nameof(mod));
        }

        RemoveMutableMod(gameMod);
    }

    public void RemoveMutableMod(TMod mod) {
        var loadOrder = Environment.LinkCache.ListedOrder
            .Where(m => m.ModKey != mod.ModKey)
            .Select(m => m.ModKey)
            .ToArray();

        BuildEnvironment(loadOrder, []);
    }

    private void BuildEnvironment(ModKey[] modKeys, IEnumerable<TMod> mutableMods) {
        var builder = GameEnvironmentBuilder<TMod, TModGetter>
            .Create(_gameReleaseContext.Release)
            .WithLoadOrder(modKeys);

        if (_resolver is not null) {
            builder = builder.WithResolver(_resolver);
        }

        foreach (var mutableMod in mutableMods) {
            builder = builder.WithOutputMod(mutableMod, OutputModTrimming.Self);
        }

        Environment = builder
            .WithOutputMod(ActiveMod, OutputModTrimming.Self)
            .Build();

        _activeModChanged.OnNext(ActiveMod.ModKey);
        _loadOrderChanged.OnNext(Environment.LoadOrder.Keys.ToList());
        _linkCacheChanged.OnNext(Environment.LinkCache);
    }
}
