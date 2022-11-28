using System.Reactive.Subjects;
using CreationEditor.Environment;
using CreationEditor.Notification;
using Elscrux.Logging;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.Core.Plugins;
using Serilog;
namespace CreationEditor.Skyrim.Environment;

public class SkyrimEditorEnvironment : IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> {
    private static readonly ModKey NewModKey = new("NewMod", ModType.Plugin);
    
    private readonly IReferenceQuery _referenceQuery;
    private readonly ISimpleEnvironmentContext _simpleEnvironmentContext;
    private readonly IBackgroundTaskManager _backgroundTaskManager;
    private readonly INotifier _notifier;
    private readonly ILogger _logger;

    private IGameEnvironment<ISkyrimMod, ISkyrimModGetter> _gameEnvironment;
    IGameEnvironment IEditorEnvironment.Environment => _gameEnvironment;
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
    
    private readonly Subject<ModKey> _activeModChanged = new();
    public IObservable<ModKey> ActiveModChanged => _activeModChanged;
    
    private readonly Subject<List<ModKey>> _loadOrderChanged = new();
    public IObservable<List<ModKey>> LoadOrderChanged => _loadOrderChanged;
    
    public SkyrimEditorEnvironment(
        IReferenceQuery referenceQuery,
        ISimpleEnvironmentContext simpleEnvironmentContext,
        IBackgroundTaskManager backgroundTaskManager,
        INotifier notifier,
        ILogger logger) {
        _referenceQuery = referenceQuery;
        _simpleEnvironmentContext = simpleEnvironmentContext;
        _backgroundTaskManager = backgroundTaskManager;
        _notifier = notifier;
        _logger = logger;

        _activeMod = new SkyrimMod(NewModKey, _simpleEnvironmentContext.GameReleaseContext.Release.ToSkyrimRelease());
        _activeModLinkCache = _activeMod.ToMutableLinkCache();
        
        _gameEnvironment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(_simpleEnvironmentContext.GameReleaseContext.Release)
            .WithLoadOrder(_activeMod.ModKey)
            .Build();
    }

    public void Build(IEnumerable<ModKey> modKeys, ModKey? activeMod = null) {
        var modKeysArray = modKeys.ToArray();

        var modsString = string.Join(' ', modKeysArray.Select(modKey => modKey.FileName));
        if (activeMod == null) {
            _logger.Here().Information("Loading mods {LoadedMods} without active mod", modsString);
        } else {
            _logger.Here().Information("Loading mods {LoadedMods} with active mod {ActiveMod}", modsString, activeMod.Value.FileName);
        }
        
        _backgroundTaskManager.ReferencesLoaded = false;
        
        _activeMod = new SkyrimMod(activeMod ?? NewModKey, _simpleEnvironmentContext.GameReleaseContext.Release.ToSkyrimRelease());
        _activeModLinkCache = _activeMod.ToMutableLinkCache();

        var linearNotifier = new LinearNotifier(_notifier, activeMod == null ? 1 : 2);
        
        if (activeMod != null) {
            linearNotifier.Next($"Preparing {activeMod.Value.FileName}");
            _activeMod.DeepCopyIn(GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
                .Create(_simpleEnvironmentContext.GameReleaseContext.Release)
                .WithLoadOrder(activeMod.Value)
                .Build()
                .LoadOrder[^1].Mod!);
        }
        
        linearNotifier.Next("Building Environment");
        _gameEnvironment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(_simpleEnvironmentContext.GameReleaseContext.Release)
            .WithLoadOrder(modKeysArray)
            .WithOutputMod(_activeMod, OutputModTrimming.Self)
            .Build();
        linearNotifier.Stop();
        
        _activeModChanged.OnNext(_activeMod.ModKey);
        _loadOrderChanged.OnNext(_gameEnvironment.LoadOrder.Select(x => x.Key).ToList());

        Prepare();
    }

    private async void Prepare() {
        var linearNotifier = new LinearNotifier(_notifier, 1);

        linearNotifier.Next("Loading References");
        await Task.Run(() => _referenceQuery.LoadModReferences(_gameEnvironment));
        _backgroundTaskManager.ReferencesLoaded = true;
        linearNotifier.Stop();
    }
}
