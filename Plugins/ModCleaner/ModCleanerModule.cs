using Autofac;
using ModCleaner.Services;
using ModCleaner.ViewModels;
namespace ModCleaner;

public class ModCleanerModule : Module {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterType<Services.ModCleaner>()
            .AsSelf();

        builder.RegisterType<ModCleanerPlugin>()
            .AsSelf();

        builder.RegisterType<ModCleanerVM>()
            .AsSelf();

        builder.RegisterType<AssetCleaner>()
            .AsSelf();

        builder.RegisterType<RecordCleaner>()
            .AsSelf();
    }
}
