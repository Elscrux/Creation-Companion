using Autofac;
using BuildStripper.Services;
using BuildStripper.Services.FeatureFlag;
using BuildStripper.ViewModels;
namespace BuildStripper;

public class BuildStripperModule : Module {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterType<Services.BuildStripper>()
            .AsSelf();

        builder.RegisterType<BuildStripperPlugin>()
            .AsSelf();

        builder.RegisterType<BuildStripperVM>()
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
