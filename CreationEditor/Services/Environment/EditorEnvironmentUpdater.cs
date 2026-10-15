using System.IO.Abstractions;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Notification;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
namespace CreationEditor.Services.Environment;

public interface IEditorEnvironmentUpdater {
    ActiveModBuilder ActiveMod { get; }
    LoadOrderBuilder LoadOrder { get; }

    internal void SetTo(IEditorEnvironment editorEnvironment);
    internal IEnumerable<ModKey> GetLoadOrder();
    IEditorEnvironmentResult Build();
}

public sealed class EditorEnvironmentUpdater<TMod, TModGetter> : IEditorEnvironmentUpdater
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IDataSourceService _dataSourceService;
    private readonly INotificationService _notificationService;
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly ILoadOrderSorter _loadOrderSorter;
    private readonly IModInfoProvider _modInfoProvider;

    // Masters of immutable mods are read from the data source files
    private readonly Dictionary<ModKey, ModKey[]> _fileMastersCache = [];

    public EditorEnvironmentUpdater(
        ILogger logger,
        IFileSystem fileSystem,
        IDataSourceService dataSourceService,
        INotificationService notificationService,
        IGameReleaseContext gameReleaseContext,
        ILoadOrderSorter loadOrderSorter,
        IModInfoProvider modInfoProvider) {
        _logger = logger;
        _fileSystem = fileSystem;
        _dataSourceService = dataSourceService;
        _notificationService = notificationService;
        _gameReleaseContext = gameReleaseContext;
        _loadOrderSorter = loadOrderSorter;
        _modInfoProvider = modInfoProvider;
        ActiveMod = new ActiveModBuilder(this);
        LoadOrder = new LoadOrderBuilder(this);
    }

    public ActiveModBuilder ActiveMod { get; }
    public LoadOrderBuilder LoadOrder { get; }

    public void SetTo(IEditorEnvironment editorEnvironment) {
        ActiveMod.LastMod = editorEnvironment.ActiveMod;
        ActiveMod.ModKey = editorEnvironment.ActiveMod.ModKey;
        foreach (var mod in editorEnvironment.GameEnvironment.LinkCache.ListedOrder.Where(x => x.ModKey != editorEnvironment.ActiveMod.ModKey)) {
            if (mod is TMod mutable) {
                LoadOrder.AddMutableMods(mutable);
            } else {
                LoadOrder.AddImmutableMods(mod.ModKey);
            }
        }
    }

    public IEnumerable<ModKey> GetLoadOrder() {
        var modKeys = LoadOrder.ImmutableMods
            .Concat(LoadOrder.MutableMods.Select(x => x.ModKey))
            .Concat(LoadOrder.NewMutableMods)
            .Where(modKey => modKey != ActiveMod.ModKey)
            .Distinct()
            .ToList();

        var result = _loadOrderSorter.Sort(modKeys, GetDirectMasters);
        foreach (var issue in result.Issues) {
            _logger.Here().Warning("Invalid load order detected: {Issue}", issue);
        }

        return result.Order.Append(ActiveMod.ModKey);
    }

    private IEnumerable<ModKey> GetDirectMasters(ModKey modKey) {
        var mod = LoadOrder.MutableMods.Find(x => x.ModKey == modKey);
        if (mod is not null) return mod.MasterReferences.Select(x => x.Master);

        if (ActiveMod.LastMod is not null && ActiveMod.LastMod.ModKey == modKey) {
            return ActiveMod.LastMod.MasterReferences.Select(x => x.Master);
        }

        if (_fileMastersCache.TryGetValue(modKey, out var cachedMasters)) return cachedMasters;

        if (_dataSourceService.TryGetFileLink(modKey.FileName.String, out var fileLink)
         && _modInfoProvider.GetModInfo(fileLink) is {} modInfo) {
            return _fileMastersCache[modKey] = modInfo.DirectMasters;
        }

        return _fileMastersCache[modKey] = [];
    }

    public IEditorEnvironmentResult Build() {
        var linearNotifier = new LinearNotifier(_notificationService, 3);

        linearNotifier.Next("Preparing immutable mods");
        var loadOrder = GetLoadOrder().ToArray();
        var builder = GameEnvironmentBuilder<TMod, TModGetter>
            .Create(_gameReleaseContext.Release)
            .WithLoadOrder(loadOrder)
            .TransformModListings(x => x.Select(TryLoadListingFromDataSource));

        linearNotifier.Next("Preparing mutable mods");
        // Output mods are placed in the environment in the order they are added, so they follow the sorted load order
        var loadOrderIndices = loadOrder.Select((key, i) => (key, i)).ToDictionary(x => x.key, x => x.i);
        foreach (var mutableMod in LoadOrder.MutableMods.OrderBy(x => loadOrderIndices.GetValueOrDefault(x.ModKey, int.MaxValue))) {
            if (mutableMod is not TMod mod) throw new ArgumentException($"Mutable mod is of type {mutableMod.GetType()} but expected {typeof(TMod)}");

            builder = builder.WithOutputMod(mod, OutputModTrimming.Self);
        }

        foreach (var newMutableMod in LoadOrder.NewMutableMods) {
            var newMod = ModFactory<TMod>.Activator(newMutableMod, _gameReleaseContext.Release, IEditorEnvironment.DefaultModVersion);
            builder = builder.WithOutputMod(newMod, OutputModTrimming.Self);
        }

        linearNotifier.Next($"Preparing active mod {ActiveMod.ModKey}");
        var activeMod = ActiveMod.Mode switch {
            ActiveModBuilder.ActiveModMode.Existing => ActiveMod.LastMod is TMod lastMod && ActiveMod.ModKey == lastMod.ModKey
                ? lastMod
                : ModFactory<TMod>.Importer(
                    new ModPath(_dataSourceService.GetFileLink(new DataRelativePath(ActiveMod.ModKey.FileName))?.FullPath
                     ?? throw new FileNotFoundException($"Active mod file {ActiveMod.ModKey.FileName} not found in any data source.")),
                    _gameReleaseContext.Release,
                    new BinaryReadParameters { FileSystem = _fileSystem }
                ),
            ActiveModBuilder.ActiveModMode.New => ModFactory<TMod>.Activator(
                ActiveMod.ModKey,
                _gameReleaseContext.Release,
                IEditorEnvironment.DefaultModVersion),
            _ => throw new InvalidOperationException(),
        };

        var environment = builder
            .WithOutputMod(activeMod, OutputModTrimming.Self);

        var result = new EditorEnvironmentResult<TMod, TModGetter>(environment, activeMod);
        _fileMastersCache.Clear();
        return result;
    }

    private IModListingGetter<TModGetter> TryLoadListingFromDataSource(IModListingGetter<TModGetter> listing) {
        if (_dataSourceService.TryGetFileLink(listing.FileName, out var link)) {
            var modPath = new ModPath(link.FullPath);
            var mod = ModFactory<TModGetter>.Importer(modPath,
                _gameReleaseContext.Release,
                new BinaryReadParameters {
                    FileSystem = _fileSystem,
                });
            return new ModListing<TModGetter>(mod, listing.Enabled, listing.GhostSuffix);
        }

        if (!listing.ModExists) {
            _logger.Here().Error("Failed to load mod listing for {FileName} from any data source {DataSources}",
                listing.FileName,
                _dataSourceService.ListedOrder.Select(x => x.Name));
        }

        return listing;
    }
}

public sealed class LoadOrderBuilder(IEditorEnvironmentUpdater updater) {
    internal readonly List<ModKey> ImmutableMods = [];
    internal readonly List<IMod> MutableMods = [];
    internal readonly List<ModKey> NewMutableMods = [];

    public IEditorEnvironmentUpdater SetImmutableMods(params IEnumerable<ModKey> modKeys) {
        ImmutableMods.ReplaceWith(modKeys);
        return updater;
    }

    public IEditorEnvironmentUpdater AddImmutableMods(params IEnumerable<ModKey> modKeys) {
        ImmutableMods.AddRange(modKeys.Select(GetFreeModKey));
        return updater;
    }

    public IEditorEnvironmentUpdater RemoveImmutableMod(ModKey modKey) {
        ImmutableMods.Remove(modKey);
        return updater;
    }

    public IEditorEnvironmentUpdater SetMutableMods(params IEnumerable<IMod> mods) {
        MutableMods.ReplaceWith(mods);
        return updater;
    }

    public IEditorEnvironmentUpdater AddMutableMods(params IEnumerable<IMod> mods) {
        MutableMods.AddRange(mods);
        return updater;
    }

    public IEditorEnvironmentUpdater RemoveMutableMod(IMod mod) {
        MutableMods.Remove(mod);
        return updater;
    }

    public IEditorEnvironmentUpdater AddNewMutableMod(ModKey modKey) {
        modKey = GetFreeModKey(modKey);

        NewMutableMods.Add(modKey);
        return updater;
    }

    private ModKey GetFreeModKey(ModKey modKey) {
        while (updater.GetLoadOrder().Contains(modKey)) {
            modKey = ModKey.FromName(modKey.Name + "_Duplicate", modKey.Type);
        }

        return modKey;
    }
}

public sealed class ActiveModBuilder(IEditorEnvironmentUpdater updater) {
    internal enum ActiveModMode { Existing, New }

    internal IMod? LastMod;
    internal ActiveModMode Mode = ActiveModMode.Existing;
    internal ModKey ModKey = ModKey.Null;
    public IEditorEnvironmentUpdater Existing(ModKey activeMod) {
        Mode = ActiveModMode.Existing;
        ModKey = activeMod;
        return updater;
    }

    public IEditorEnvironmentUpdater New(string newModName, ModType modType = ModType.Plugin) {
        Mode = ActiveModMode.New;
        ModKey = new ModKey(newModName, modType);
        return updater;
    }
}
