using Autofac;
using QueryPlugin.ViewModels;
namespace QueryPlugin;

public class QueryPluginModule : Module {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterType<QueryPlugin>()
            .AsSelf();

        builder.RegisterType<QueryPluginVM>()
            .AsSelf();

        builder.RegisterType<QueryColumnVM>()
            .AsSelf();
    }
}
