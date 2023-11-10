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

    public IGameEnvironment<ISkyrimMod, ISkyrimModGetter> Environment { get; set; }
    IGameEnvironment IEditorEnvironment.GameEnvironment => Environment;

    public ILinkCache LinkCache => Environment.LinkCache;

    public ISkyrimMod ActiveMod { get; set; }
    IMod IEditorEnvironment.ActiveMod => ActiveMod;

    public ILinkCache<ISkyrimMod, ISkyrimModGetter> ActiveModLinkCache { get; set; }
    ILinkCache IEditorEnvironment.ActiveModLinkCache => ActiveModLinkCache;

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

        ActiveMod = new SkyrimMod(ModKey.Null, _gameReleaseContext.Release.ToSkyrimRelease());
        ActiveModLinkCache = ActiveMod.ToMutableLinkCache();

        Environment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(_gameReleaseContext.Release)
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
        ActiveMod = ModInstantiator<ISkyrimMod>.Importer(activeModPath, _gameReleaseContext.Release, _fileSystem);
        ActiveModLinkCache = ActiveMod.ToMutableLinkCache();

        linearNotifier.Next("Building GameEnvironment");

        BuildEnvironment(modKeysArray);

        linearNotifier.Stop();
    }

    public void Build(IEnumerable<ModKey> modKeys, string newModName, ModType modType) {
        var modKeysArray = modKeys.ToArray();

        var modsString = string.Join(' ', modKeysArray.Select(modKey => modKey.FileName));
        _logger.Here().Information("Loading mods {LoadedMods} without active mod", modsString);

        ActiveMod = new SkyrimMod(new ModKey(newModName, modType), _gameReleaseContext.Release.ToSkyrimRelease());
        ActiveModLinkCache = ActiveMod.ToMutableLinkCache();

        var linearNotifier = new LinearNotifier(_notificationService);
        linearNotifier.Next("Building GameEnvironment");

        BuildEnvironment(modKeysArray);

        linearNotifier.Stop();
    }

    private void BuildEnvironment(ModKey[] modKeys) {
        Environment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(_gameReleaseContext.Release)
            .WithLoadOrder(modKeys)
            .WithOutputMod(ActiveMod, OutputModTrimming.Self)
            .Build();

        _activeModChanged.OnNext(ActiveMod.ModKey);
        _loadOrderChanged.OnNext(Environment.LoadOrder.Select(x => x.Key).ToList());
        _linkCacheChanged.OnNext(Environment.LinkCache);
    }
}
