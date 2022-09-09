using Elscrux.Notification;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.Core.Plugins;
using MutagenLibrary.References.ReferenceCache;
namespace CreationEditor.Services.Environment;

public interface IEditorEnvironment {
    public IGameEnvironment<ISkyrimMod, ISkyrimModGetter> Environment { get; set; }
    public ILinkCache<ISkyrimMod, ISkyrimModGetter> LinkCache => Environment.LinkCache;
    public IEnumerable<ModKey> LoadedMods => Environment.LoadOrder.Keys;
    
    public SkyrimMod ActiveMod { get; set; }
    public ILinkCache ActiveModLinkCache { get; set; }

    public void Build(IEnumerable<ModKey> modKeys, ModKey? activeMod = null);
    
    public event EventHandler? ActiveModChanged;
    public event EventHandler? EditorInitialized;
}

public class EditorEnvironment : IEditorEnvironment {
    private readonly IReferenceQuery _referenceQuery;
    private readonly ISimpleEnvironmentContext _simpleEnvironmentContext;
    private readonly INotifier _notifier;
    
    public IGameEnvironment<ISkyrimMod, ISkyrimModGetter> Environment { get; set; }
    public SkyrimMod ActiveMod { get; set; }
    public ILinkCache ActiveModLinkCache { get; set; }

    public event EventHandler? ActiveModChanged;
    public event EventHandler? EditorInitialized;
    
    public EditorEnvironment(
        IReferenceQuery referenceQuery,
        ISimpleEnvironmentContext simpleEnvironmentContext,
        INotifier notifier) {
        _referenceQuery = referenceQuery;
        _simpleEnvironmentContext = simpleEnvironmentContext;
        _notifier = notifier;

        ActiveMod = new SkyrimMod(ModKey.Null, _simpleEnvironmentContext.GameReleaseContext.Release.ToSkyrimRelease());
        ActiveModLinkCache = ActiveMod.ToMutableLinkCache();
        
        Environment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(_simpleEnvironmentContext.GameReleaseContext.Release)
            .WithLoadOrder(ActiveMod.ModKey)
            .Build();
    }

    public void Build(IEnumerable<ModKey> modKeys, ModKey? activeMod = null) {
        ActiveMod = new SkyrimMod(activeMod ?? ModKey.Null, _simpleEnvironmentContext.GameReleaseContext.Release.ToSkyrimRelease());

        var linearNotifier = new LinearNotifier(_notifier, activeMod == null ? 2 : 3);
        
        if (activeMod != null) {
            linearNotifier.Next($"Preparing {activeMod.Value.FileName}");
            ActiveMod.DeepCopyIn(GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
                .Create(_simpleEnvironmentContext.GameReleaseContext.Release)
                .WithLoadOrder(activeMod.Value)
                .Build()
                .LoadOrder[^1].Mod!);
        }
        
        linearNotifier.Next("Building Environment");
        Environment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(_simpleEnvironmentContext.GameReleaseContext.Release)
            .WithLoadOrder(modKeys.ToArray())
            .WithOutputMod(ActiveMod, OutputModTrimming.Self)
            .Build();

        linearNotifier.Next("Loading References");
        _referenceQuery.LoadModReferences(Environment);
        
        ActiveModChanged?.Invoke(this, EventArgs.Empty);
        EditorInitialized?.Invoke(this, EventArgs.Empty);
    }
}
