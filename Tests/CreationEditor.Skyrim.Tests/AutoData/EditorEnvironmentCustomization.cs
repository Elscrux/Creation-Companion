using System.IO.Abstractions;
using AutoFixture;
using AutoFixture.Kernel;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Notification;
using CreationEditor.Services.Serialization.Json;
using CreationEditor.Services.State;
using CreationEditor.Skyrim.Services.Asset;
using CreationEditor.Skyrim.Services.Mod;
using CreationEditor.Skyrim.Services.Types;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Newtonsoft.Json.Serialization;
using Noggog;
using Noggog.Testing.AutoFixture;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.AutoData;

public sealed class EditorEnvironmentCustomization(
    bool configureMembers = false,
    GameRelease release = GameRelease.SkyrimSE,
    TargetFileSystem targetFileSystem = TargetFileSystem.Fake) : ICustomization {

    private const string DataDirectoryPath = @"C:\GameDirectory\Data";

    public void Customize(IFixture fixture) {
        // Mutagen customizations
        var mutagenDefaultCustomization = new MutagenDefaultCustomization(configureMembers, release, targetFileSystem);
        mutagenDefaultCustomization.Customize(fixture);
        var mutagenConcreteModsCustomization = new MutagenConcreteModsCustomization(release, configureMembers);
        mutagenConcreteModsCustomization.Customize(fixture);

        // Custom customizations
        fixture.Register<INotificationService>(fixture.Create<NotificationService>);
        fixture.Register<ILoadOrderSorter>(fixture.Create<LoadOrderSorter>);
        fixture.Register<IMutagenCommonAspectsProvider>(fixture.Create<SkyrimCommonAspectsProvider>);
        fixture.Register<IModInfoProvider>(fixture.Create<SkyrimModInfoProvider>);
        fixture.Register<IModService>(fixture.Create<ModService>);
        fixture.Register<IMutagenTypeProvider>(fixture.Create<MutagenTypeProvider>);
        fixture.Register<IAssetTypeProvider>(fixture.Create<SkyrimAssetTypeProvider>);
        fixture.Register<IAssetTypeService>(fixture.Create<AssetTypeService>);
        fixture.Register<IModelModificationService>(fixture.Create<NifModificationService>);
        fixture.Register<Func<IReadOnlyList<string>, ICacheLocationProvider>>(() =>
            subDirectories => new CacheLocationProvider(fixture.Create<IFileSystem>(), subDirectories));
        fixture.Register<IReferenceCacheSerializationConfig<IModGetter, IFormLinkIdentifier>>(
            fixture.Create<ModAssetReferenceCacheSerializationConfig>);
        fixture.Register<Func<IEditorEnvironmentUpdater>>(() => fixture.Create<EditorEnvironmentUpdater<ISkyrimMod, ISkyrimModGetter>>);

        // AutoFixture creates Func<T> delegates that return null - register factories that create real triggers
        RegisterTriggerFactory(fixture,
            () => fixture.Create<ModReferenceUpdateTrigger<DictionaryReferenceCache<string, IFormLinkIdentifier>, string, IReferencedRecord>>());
        RegisterTriggerFactory(fixture, () => fixture.Create<ModReferenceUpdateTrigger<RecordReferenceCache, IFormLinkIdentifier, IReferencedRecord>>());
        RegisterTriggerFactory(fixture, () => fixture.Create<ModReferenceUpdateTrigger<AssetReferenceCache<IFormLinkIdentifier>, IAssetLinkGetter, IReferencedAsset>>());
        RegisterTriggerFactory(fixture,
            () => fixture.Create<DataSourceReferenceUpdateTrigger<AssetReferenceCache<DataRelativePath>, IAssetLinkGetter, IReferencedAsset>>());
        RegisterTriggerFactory(fixture, () => fixture.Create<DataSourceReferenceUpdateTrigger<AssetDictionaryReferenceCache<uint>, uint, IReferencedRecord>>());
        RegisterTriggerFactory(fixture, () => fixture.Create<DataSourceReferenceUpdateTrigger<AssetDictionaryReferenceCache<string>, string, IReferencedRecord>>());

        fixture.Customizations.Add(new SingletonBuilder<IRecordController, RecordController<ISkyrimMod, ISkyrimModGetter>>(fixture));
        fixture.Customizations.Add(new SingletonBuilder<IReferenceService, ReferenceService>(fixture));
        fixture.Customizations.Add(new SingletonBuilder<IAssetController, AssetController>(fixture));

        var builder = fixture.Create<Func<IEditorEnvironmentUpdater>>();
        var gameReleaseContext = fixture.Create<IGameReleaseContext>();
        var gameEnvironment = fixture.Create<IGameEnvironment>();
        var editorEnvironment = new EditorEnvironment<ISkyrimMod, ISkyrimModGetter>(
            builder,
            gameReleaseContext,
            Logger.None,
            t => fixture.Create(t, new SpecimenContext(fixture)));

        fixture.Inject<IEditorEnvironment>(editorEnvironment);
        fixture.Inject<IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>>(editorEnvironment);
        fixture.Register<ILinkCacheProvider>(() => editorEnvironment);

        // Real services over the mock file system
        var fileSystem = fixture.Create<IFileSystem>();
        var dataDirectoryProvider = new DataDirectoryInjection(new DirectoryPath(DataDirectoryPath));
        var archiveService = new ArchiveService(
            fileSystem,
            new GetArchiveIniListings(fileSystem, new IniPathInjection(new FilePath(@"C:\GameDirectory\Skyrim.ini"))),
            new ArchiveExtensionProvider(gameReleaseContext),
            editorEnvironment);
        var jsonSerializerSettings = new JsonSerializerSettingsProvider(new DefaultContractResolver());
        StateRepositoryFactory<DataSourceMemento, DataSourceMemento, NamedGuid> stateRepositoryFactory = null!;
        stateRepositoryFactory = new StateRepositoryFactory<DataSourceMemento, DataSourceMemento, NamedGuid>(
            stateIds => new JsonStateRepository<DataSourceMemento, DataSourceMemento, NamedGuid>(
                jsonSerializerSettings,
                new NamedGuidStateIdentifier(),
                Logger.None,
                fileSystem,
                stateIds),
            stateIds => new CachedStateRepository<DataSourceMemento, DataSourceMemento, NamedGuid>(stateIds, stateRepositoryFactory));
        var dataSourceService = new DataSourceService(fileSystem, dataDirectoryProvider, archiveService, stateRepositoryFactory);

        fixture.Inject<IDataDirectoryProvider>(dataDirectoryProvider);
        fixture.Inject<IArchiveService>(archiveService);
        fixture.Inject<IStateRepositoryFactory<DataSourceMemento, DataSourceMemento, NamedGuid>>(stateRepositoryFactory);
        fixture.Inject<IDataSourceService>(dataSourceService);

        editorEnvironment.Update(updater => updater
            .LoadOrder.AddImmutableMods(gameEnvironment.LinkCache.ListedOrder.Select(x => x.ModKey))
            .ActiveMod.New("NewMod")
            .Build());
    }

    private static void RegisterTriggerFactory<TTrigger>(IFixture fixture, Func<TTrigger> factory) {
        fixture.Register<Func<TTrigger>>(() => factory);
    }
}
