using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.Notification;
using MutagenLibrary.ReferenceCache;
namespace CreationEditor; 

public class Editor {
    public static Editor Instance = new();

    public static readonly ReferenceQuery ReferenceQuery = new();
    
    public readonly IGameEnvironment<ISkyrimMod, ISkyrimModGetter> Environment;
    public ILinkCache<ISkyrimMod, ISkyrimModGetter> LinkCache => Environment.LinkCache;

    public IEnumerable<ModKey> LoadedMods => Environment.LoadOrder.Keys;

    public readonly SkyrimMod ActiveMod;

    public static event EventHandler? ActiveModChanged;
    public static event EventHandler? EditorInitialized;

    //Null editor constructor
    private Editor() {
        ActiveMod = new SkyrimMod(ModKey.Null, Constants.SkyrimRelease);

        Environment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(Constants.GameRelease)
            .WithLoadOrder(ActiveMod.ModKey)
            .Build();
    }
    
    private Editor(IEnumerable<ModKey> modKeys, ModKey? activeMod = null, INotifier? notifier = null) {
        ActiveMod = new SkyrimMod(activeMod ?? ModKey.Null, Constants.SkyrimRelease);
        if (activeMod != null) {
            notifier?.Notify($"Preparing {activeMod.Value.FileName}");
            ActiveMod.DeepCopyIn(GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
                .Create(Constants.GameRelease)
                .WithLoadOrder(activeMod.Value)
                .Build()
                .LoadOrder[^1].Mod!);
        }
        
        notifier?.Notify("Building Environment");
        Environment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(Constants.GameRelease)
            .WithLoadOrder(modKeys.ToArray())
            .WithOutputMod(ActiveMod, OutputModTrimming.Self)
            .Build();

        notifier?.Notify("Loading References");
        // ReferenceQuery.LoadModReferences(Environment, notifier);// todo reenable
    }
    
    public static void Build(IEnumerable<ModKey> modKeys, ModKey? activeMod = null, INotifier? load = null) {
        Instance = new Editor(modKeys, activeMod, load);
        ActiveModChanged?.Invoke(Instance, EventArgs.Empty);
        EditorInitialized?.Invoke(Instance, EventArgs.Empty);
    }
}
