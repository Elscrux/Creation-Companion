using Autofac;
using BSAssetsTrimmer.ViewModels;
namespace BSAssetsTrimmer;

public class ModCleanerModule : Module {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterType<Services.BSAssetsTrimmer>()
            .AsSelf();

        builder.RegisterType<BSAssetsTrimmerPlugin>()
            .AsSelf();

        builder.RegisterType<BSAssetsTrimmerVM>()
            .AsSelf();
    }
}
