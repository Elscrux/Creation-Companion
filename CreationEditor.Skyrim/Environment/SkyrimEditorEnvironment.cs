using CreationEditor.Environment;
using Elscrux.Notification;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.Core.Plugins;
using MutagenLibrary.References.ReferenceCache;
namespace CreationEditor.Skyrim.Environment;

public class SkyrimEditorEnvironment : IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> {
    private readonly IReferenceQuery _referenceQuery;
    private readonly ISimpleEnvironmentContext _simpleEnvironmentContext;
    private readonly INotifier _notifier;

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

    public event EventHandler? ActiveModChanged;
    public event EventHandler? EditorInitialized;
    
    public SkyrimEditorEnvironment(
        IReferenceQuery referenceQuery,
        ISimpleEnvironmentContext simpleEnvironmentContext,
        INotifier notifier) {
        _referenceQuery = referenceQuery;
        _simpleEnvironmentContext = simpleEnvironmentContext;
        _notifier = notifier;

        _activeMod = new SkyrimMod(ModKey.Null, _simpleEnvironmentContext.GameReleaseContext.Release.ToSkyrimRelease());
        _activeModLinkCache = _activeMod.ToMutableLinkCache();
        
        _gameEnvironment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(_simpleEnvironmentContext.GameReleaseContext.Release)
            .WithLoadOrder(_activeMod.ModKey)
            .Build();
    }

    public void Build(IEnumerable<ModKey> modKeys, ModKey? activeMod = null) {
        _activeMod = new SkyrimMod(activeMod ?? ModKey.Null, _simpleEnvironmentContext.GameReleaseContext.Release.ToSkyrimRelease());

        var linearNotifier = new LinearNotifier(_notifier, activeMod == null ? 2 : 3);
        
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
            .WithLoadOrder(modKeys.ToArray())
            .WithOutputMod(_activeMod, OutputModTrimming.Self)
            .Build();

        linearNotifier.Next("Loading References");
        _referenceQuery.LoadModReferences(_gameEnvironment);
        
        ActiveModChanged?.Invoke(this, EventArgs.Empty);
        EditorInitialized?.Invoke(this, EventArgs.Empty);
    }
}
