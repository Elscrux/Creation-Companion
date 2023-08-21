using Autofac;
using SearchPlugin.ViewModels;
namespace SearchPlugin; 

public class SearchPluginModule : Module {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterGeneric(typeof(SearchPlugin<,>))
            .AsSelf();

        builder.RegisterGeneric(typeof(TextSearchVM<,>))
            .AsSelf();
    }
}
