using Autofac;
using CreationEditor.Services.Plugin;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Modules;

public sealed class GameSpecificModule<TMod, TModGetter> : Module
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterType<AutofacPluginService<TMod, TModGetter>>()
            .As<IPluginService>()
            .SingleInstance();
    }
}
