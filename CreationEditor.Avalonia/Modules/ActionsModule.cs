using Autofac;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Actions.Global;
namespace CreationEditor.Avalonia.Modules;

public sealed class ActionsModule : Module {
    protected override void Load(ContainerBuilder builder) {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Global Actions
        builder.RegisterType<GlobalActionRegistry>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterAssemblyTypes(assemblies)
            .AssignableTo<IGlobalActionsProvider>()
            .As<IGlobalActionsProvider>()
            .SingleInstance();

        // Context Actions
        builder.RegisterAssemblyTypes(assemblies)
            .AssignableTo<IContextActionsProvider>()
            .As<IContextActionsProvider>()
            .SingleInstance();

        builder.RegisterType<InjectedContextMenuProvider>()
            .As<IContextMenuProvider>();
    }
}
