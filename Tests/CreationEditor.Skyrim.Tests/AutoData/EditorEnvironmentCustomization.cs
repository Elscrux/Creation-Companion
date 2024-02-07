using System.IO.Abstractions;
using AutoFixture;
using AutoFixture.Kernel;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record.Cache;
using CreationEditor.Services.Notification;
using CreationEditor.Skyrim.Tests.Mock;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.AutoData;

public sealed class EditorEnvironmentCustomization(
    bool configureMembers = false,
    GameRelease release = GameRelease.SkyrimSE,
    bool useMockFileSystem = true) : ICustomization {

    public void Customize(IFixture fixture) {
        var mutagenDefaultCustomization = new MutagenDefaultCustomization(configureMembers, release, useMockFileSystem);
        mutagenDefaultCustomization.Customize(fixture);

        fixture.Register<INotificationService>(fixture.Create<NotificationService>);
        fixture.Register<IRecordReferenceCacheFactory>(() => new EmptyRecordReferenceCacheFactory(fixture));
        fixture.Customizations.Add(new SingletonBuilder<IRecordController, RecordController<ISkyrimMod, ISkyrimModGetter>>(fixture));

        var fileSystem = fixture.Create<IFileSystem>();
        var gameReleaseContext = fixture.Create<IGameReleaseContext>();
        var dataDirectoryProvider = fixture.Create<IDataDirectoryProvider>();
        var gameEnvironment = fixture.Create<IGameEnvironment>();
        var editorEnvironment = new EditorEnvironment<ISkyrimMod, ISkyrimModGetter>(fileSystem, gameReleaseContext, dataDirectoryProvider, new NotificationService(), Logger.None, t => fixture.Create(t, new SpecimenContext(fixture)));
        editorEnvironment.Build(gameEnvironment.LoadOrder.Keys, "NewMod", ModType.Plugin);
        fixture.Inject<IEditorEnvironment>(editorEnvironment);
        fixture.Inject<IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>>(editorEnvironment);
    }
}
