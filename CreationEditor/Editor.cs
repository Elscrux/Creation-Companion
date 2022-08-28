using System.Runtime.Serialization;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.Notification;
using MutagenLibrary.ReferenceCache;
namespace CreationEditor; 

public class Editor {
    public static Editor Instance = new();

    public readonly IGameEnvironment<ISkyrimMod, ISkyrimModGetter> Environment;
    public ILinkCache<ISkyrimMod, ISkyrimModGetter> LinkCache;
    public readonly ReferenceQuery ReferenceQuery;

    public readonly IEnumerable<ModKey> LoadedMods;

    public readonly SkyrimMod ActiveMod;

    public static event EventHandler? ActiveModChanged;
    public static event EventHandler? EditorFinishLoading;

    //Null editor constructor
    private Editor() {
        ActiveMod = new SkyrimMod(ModKey.Null, Constants.SkyrimRelease);
        Environment = (IGameEnvironment<ISkyrimMod, ISkyrimModGetter>) FormatterServices.GetUninitializedObject(typeof(GameEnvironmentState<ISkyrimMod, ISkyrimModGetter>));
        LinkCache = ActiveMod.ToImmutableLinkCache();
        LoadedMods = new List<ModKey>();
        ReferenceQuery = new ReferenceQuery();
    }
    
    private Editor(IEnumerable<ModKey> modKeys, ModKey? activeMod = null, INotifier? notifier = null) {
        ActiveModChanged?.Invoke(this, EventArgs.Empty);
        
        notifier?.Notify("Building Environment");
        
        ActiveMod = new SkyrimMod(activeMod ?? ModKey.Null, Constants.SkyrimRelease);
        if (activeMod != null) {
            ActiveMod.DeepCopyIn(GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
                .Create(Constants.GameRelease)
                .WithLoadOrder(activeMod.Value)
                .Build()
                .LoadOrder[^1].Mod!);
        }
        
        Environment = GameEnvironmentBuilder<ISkyrimMod, ISkyrimModGetter>
            .Create(Constants.GameRelease)
            .WithLoadOrder(modKeys.ToArray())
            .WithOutputMod(ActiveMod, OutputModTrimming.Self)
            .Build();

        LinkCache = Environment.LinkCache;
        LoadedMods = Environment.LoadOrder.Keys;
        
        notifier?.Notify("Loading References");

        ReferenceQuery = new ReferenceQuery(Environment, notifier);
        
        EditorFinishLoading?.Invoke(this, EventArgs.Empty);
    }
    
    public static void Build(IEnumerable<ModKey> modKeys, ModKey? activeMod = null, INotifier? load = null) => Instance = new Editor(modKeys, activeMod, load);
}
