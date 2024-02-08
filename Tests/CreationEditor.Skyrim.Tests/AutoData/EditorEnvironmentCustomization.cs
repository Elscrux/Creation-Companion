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
        fixture.Register<Func<IEditorEnvironmentUpdater>>(() => fixture.Create<EditorEnvironmentUpdater<ISkyrimMod, ISkyrimModGetter>>);
        fixture.Customizations.Add(new SingletonBuilder<IRecordController, RecordController<ISkyrimMod, ISkyrimModGetter>>(fixture));

        var builder = fixture.Create<Func<IEditorEnvironmentUpdater>>();
        var gameReleaseContext = fixture.Create<IGameReleaseContext>();
        var gameEnvironment = fixture.Create<IGameEnvironment>();
        var editorEnvironment = new EditorEnvironment<ISkyrimMod, ISkyrimModGetter>(builder, gameReleaseContext, Logger.None, t => fixture.Create(t, new SpecimenContext(fixture)));
        editorEnvironment.Update(updater => updater
            .LoadOrder.AddImmutableMods(gameEnvironment.LinkCache.ListedOrder.Select(x => x.ModKey))
            .ActiveMod.New("NewMod")
            .Build());

        fixture.Inject<IEditorEnvironment>(editorEnvironment);
        fixture.Inject<IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>>(editorEnvironment);
    }
}
