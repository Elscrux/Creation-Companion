using Autofac;
using CreationEditor.Services.Plugin;
using PromoteToMaster.ViewModels;
namespace PromoteToMaster;

public class PromoteToMasterModule : ExtensionModule {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<PromoteToMasterPlugin>()
            .AsSelf();

        builder.RegisterType<PromoteToMasterVM>()
            .AsSelf();
    }
}
