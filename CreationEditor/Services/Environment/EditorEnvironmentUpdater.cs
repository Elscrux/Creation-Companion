﻿using System.IO.Abstractions;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Notification;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
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
    private readonly IFileSystem _fileSystem;
    private readonly IDataSourceService _dataSourceService;
    private readonly INotificationService _notificationService;
    private readonly IGameReleaseContext _gameReleaseContext;

    public EditorEnvironmentUpdater(
        IFileSystem fileSystem,
        IDataSourceService dataSourceService,
        INotificationService notificationService,
        IGameReleaseContext gameReleaseContext) {
        _fileSystem = fileSystem;
        _dataSourceService = dataSourceService;
        _notificationService = notificationService;
        _gameReleaseContext = gameReleaseContext;
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
        return LoadOrder.ImmutableMods
            .Concat(LoadOrder.MutableMods.Select(x => x.ModKey))
            .Concat(LoadOrder.NewMutableMods)
            .Where(modKey => modKey != ActiveMod.ModKey)
            .Append(ActiveMod.ModKey)
            .Distinct();
    }

    public IEditorEnvironmentResult Build() {
        var linearNotifier = new LinearNotifier(_notificationService, 3);

        linearNotifier.Next("Preparing immutable mods");
        var builder = GameEnvironmentBuilder<TMod, TModGetter>
            .Create(_gameReleaseContext.Release)
            .WithLoadOrder(GetLoadOrder().ToArray())
            .TransformModListings(x => x.Select(TryLoadListingFromFallback));

        linearNotifier.Next("Preparing mutable mods");
        foreach (var mutableMod in LoadOrder.MutableMods) {
            if (mutableMod is not TMod mod) throw new ArgumentException($"Mutable mod is of type {mutableMod.GetType()} but expected {typeof(TMod)}");

            builder = builder.WithOutputMod(mod, OutputModTrimming.Self);
        }

        foreach (var newMutableMod in LoadOrder.NewMutableMods) {
            var newMod = ModInstantiator<TMod>.Activator(newMutableMod, _gameReleaseContext.Release, IEditorEnvironment.DefaultModVersion);
            builder = builder.WithOutputMod(newMod, OutputModTrimming.Self);
        }

        linearNotifier.Next($"Preparing active mod {ActiveMod.ModKey}");
        var activeMod = ActiveMod.Mode switch {
            ActiveModBuilder.ActiveModMode.Existing => ActiveMod.LastMod is TMod lastMod && ActiveMod.ModKey == lastMod.ModKey
                ? lastMod
                : ModInstantiator<TMod>.Importer(
                    new ModPath(_dataSourceService.GetFileLink(new DataRelativePath(ActiveMod.ModKey.FileName))?.FullPath
                     ?? throw new FileNotFoundException($"Active mod file {ActiveMod.ModKey.FileName} not found in any data source.")),
                    _gameReleaseContext.Release,
                    new BinaryReadParameters { FileSystem = _fileSystem }
                ),
            ActiveModBuilder.ActiveModMode.New => ModInstantiator<TMod>.Activator(
                ActiveMod.ModKey,
                _gameReleaseContext.Release,
                IEditorEnvironment.DefaultModVersion),
            _ => throw new InvalidOperationException(),
        };

        var environment = builder
            .WithOutputMod(activeMod, OutputModTrimming.Self);

        return new EditorEnvironmentResult<TMod, TModGetter>(environment, activeMod);
    }

    private IModListingGetter<TModGetter> TryLoadListingFromFallback(IModListingGetter<TModGetter> listing) {
        if (listing.ExistsOnDisk) return listing;

        foreach (var fallbackModLocation in LoadOrder.FallbackModLocations) {
            var fileName = _fileSystem.Path.GetFileName(fallbackModLocation);
            if (fileName != listing.FileName) continue;

            var modPath = new ModPath(fallbackModLocation);
            var mod = ModInstantiator<TModGetter>.Importer(modPath,
                _gameReleaseContext.Release,
                new BinaryReadParameters {
                    FileSystem = _fileSystem
                });
            return new ModListing<TModGetter>(mod, listing.Enabled, listing.GhostSuffix);
        }

        return listing;
    }
}

public sealed class LoadOrderBuilder(IEditorEnvironmentUpdater updater) {
    internal readonly List<ModKey> ImmutableMods = [];
    internal readonly List<IMod> MutableMods = [];
    internal readonly List<ModKey> NewMutableMods = [];
    internal readonly List<string> FallbackModLocations = [];

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

    public IEditorEnvironmentUpdater AddFallbackModLocation(IEnumerable<string> modLocations) {
        FallbackModLocations.AddRange(modLocations);
        return updater;
    }

    public IEditorEnvironmentUpdater RemoveFallbackModLocation(string modLocation) {
        FallbackModLocations.Remove(modLocation);
        return updater;
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
