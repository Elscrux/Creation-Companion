using Autofac;
using WaterPlugin.Services;
using WaterPlugin.ViewModels;
namespace WaterPlugin;

public class WaterPluginModule : Module {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterGeneric(typeof(WaterPlugin<,>))
            .AsSelf();

        builder.RegisterType<WaterMapVM>()
            .AsSelf();

        builder.RegisterType<WaterGradientGenerator>()
            .AsSelf();
    }
}
