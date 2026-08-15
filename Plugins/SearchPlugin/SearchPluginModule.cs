using Autofac;
using SearchPlugin.Models;
using SearchPlugin.ViewModels;
using Activator = SearchPlugin.Models.Activator;
namespace SearchPlugin;

public class SearchPluginModule : Module {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterGeneric(typeof(SearchPlugin<,>))
            .AsSelf();

        builder.RegisterGeneric(typeof(TextSearchVM<,>))
            .AsSelf();

        builder.RegisterType<Activator>();
        builder.RegisterType<Book>();
        builder.RegisterType<EditorID>();
        builder.RegisterType<LoadScreen>();
        builder.RegisterType<Message>();
        builder.RegisterType<Name>();
        builder.RegisterType<Npc>();
        builder.RegisterType<Quest>();
        builder.RegisterType<Topic>();
    }
}
