using Autofac;
using CreationEditor.Services.Plugin;
using NifPlugin.Services;
using NifPlugin.ViewModels;
namespace NifPlugin;

public class NifModule : ExtensionModule {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterType<NifPlugin>()
            .AsSelf();

        builder.RegisterType<NifVM>()
            .AsSelf();

        builder.RegisterType<NifEditVertexColorService>()
            .AsSelf();

        builder.RegisterType<NifVMFactory>()
            .SingleInstance();
    }
}
