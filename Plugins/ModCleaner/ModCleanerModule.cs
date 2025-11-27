using Autofac;
using ModCleaner.Services;
using ModCleaner.Services.FeatureFlag;
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

        builder.RegisterType<FeatureFlagEditorVM>()
            .AsSelf();

        builder.RegisterType<AssetCleaner>()
            .AsSelf();

        builder.RegisterType<RecordCleaner>()
            .AsSelf();

        builder.RegisterType<FeatureFlagService>()
            .AsImplementedInterfaces()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<EssentialRecordProvider>()
            .AsImplementedInterfaces()
            .AsSelf()
            .SingleInstance();
    }
}
