using Autofac;
using VanillaDuplicateCleaner.ViewModels;
namespace VanillaDuplicateCleaner;

public class VanillaDuplicateCleanerModule : Module {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterType<VanillaDuplicateCleanerPlugin>()
            .AsSelf();

        builder.RegisterType<VanillaDuplicateCleanerVM>()
            .AsSelf();
    }
}
