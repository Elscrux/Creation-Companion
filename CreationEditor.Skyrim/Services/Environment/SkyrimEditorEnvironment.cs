using System.IO.Abstractions;
using System.Reactive.Subjects;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Notification;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Serilog;
namespace CreationEditor.Skyrim.Services.Environment;

public sealed class SkyrimEditorEnvironment : IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> {
    private readonly IFileSystem _fileSystem;
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    private IGameEnvironment<ISkyrimMod, ISkyrimModGetter> _gameEnvironment;
    IGameEnvironment IEditorEnvironment.GameEnvironment => _gameEnvironment;
    IGameEnvironment<ISkyrimMod, ISkyrimModGetter> IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>.Environment {
        get => _gameEnvironment;
        set => _gameEnvironment = value;
    }

    private ISkyrimMod _activeMod;
    IMod IEditorEnvironment.ActiveMod => _activeMod;
    ISkyrimMod IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>.ActiveMod {
        get => _activeMod;
        set => _activeMod = value;
    }

    private ILinkCache<ISkyrimMod, ISkyrimModGetter> _activeModLinkCache;
    public ILinkCache ActiveModLinkCache => _activeModLinkCache;
    ILinkCache<ISkyrimMod, ISkyrimModGetter> IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>.ActiveModLinkCache {
        get => _activeModLinkCache;
        set => _activeModLinkCache = value;
    }

    private readonly ReplaySubject<ModKey> _activeModChanged = new(1);
    public IObservable<ModKey> ActiveModChanged => _activeModChanged;

    private readonly ReplaySubject<List<ModKey>> _loadOrderChanged = new(1);
    public IObservable<List<ModKey>> LoadOrderChanged => _loadOrderChanged;

    private readonly ReplaySubject<ILinkCache> _linkCacheChanged = new(1);
    public IObservable<ILinkCache> LinkCacheChanged => _linkCacheChanged;

    public SkyrimEditorEnvironment(
        IFileSystem fileSystem,
        IGameReleaseContext gameReleaseContext,
        IDataDirectoryProvider dataDirectoryProvider,
        INotificationService notificationService,
        ILogger logger) {
        _fileSystem = fileSystem;
        _gameReleaseContext = gameReleaseContext;
        _dataDirectoryProvider = dataDirectoryProvider;
        _notificationService = notificationService;
        _logger = logger;

        _activeMod = new SkyrimMod(ModKey.Null, _gameReleaseContext.Release.ToSkyrimRelease());
        _activeModLinkCache = _activeMod.ToMutableLinkCache();

        _gameEnvironment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(_gameReleaseContext.Release)
            .WithLoadOrder(_activeMod.ModKey)
            .Build();
    }

    public void Build(IEnumerable<ModKey> modKeys, ModKey activeMod) {
        var modKeysArray = modKeys.ToArray();

        var modsString = string.Join(' ', modKeysArray.Select(modKey => modKey.FileName));
        _logger.Here().Information("Loading mods {LoadedMods} with active mod {ActiveMod}", modsString, activeMod.FileName);

        var linearNotifier = new LinearNotifier(_notificationService, 2);

        linearNotifier.Next($"Preparing {activeMod.FileName}");
        var activeModPath = new ModPath(_fileSystem.Path.Combine(_dataDirectoryProvider.Path, activeMod.FileName));
        _activeMod = ModInstantiator<ISkyrimMod>.Importer(activeModPath, _gameReleaseContext.Release, _fileSystem);
        _activeModLinkCache = _activeMod.ToMutableLinkCache();

        linearNotifier.Next("Building GameEnvironment");

        BuildEnvironment(modKeysArray);

        linearNotifier.Stop();
    }

    public void Build(IEnumerable<ModKey> modKeys, string newModName, ModType modType) {
        var modKeysArray = modKeys.ToArray();

        var modsString = string.Join(' ', modKeysArray.Select(modKey => modKey.FileName));
        _logger.Here().Information("Loading mods {LoadedMods} without active mod", modsString);

        _activeMod = new SkyrimMod(new ModKey(newModName, modType), _gameReleaseContext.Release.ToSkyrimRelease());
        _activeModLinkCache = _activeMod.ToMutableLinkCache();

        var linearNotifier = new LinearNotifier(_notificationService);
        linearNotifier.Next("Building GameEnvironment");

        BuildEnvironment(modKeysArray);

        linearNotifier.Stop();
    }

    private void BuildEnvironment(ModKey[] modKeys) {
        _gameEnvironment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(_gameReleaseContext.Release)
            .WithLoadOrder(modKeys)
            .WithOutputMod(_activeMod, OutputModTrimming.Self)
            .Build();

        _activeModChanged.OnNext(_activeMod.ModKey);
        _loadOrderChanged.OnNext(_gameEnvironment.LoadOrder.Select(x => x.Key).ToList());
        _linkCacheChanged.OnNext(_gameEnvironment.LinkCache);
    }
}
