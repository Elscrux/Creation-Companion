using Autofac;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.Services.Docking.Definitions;
namespace CreationEditor.Avalonia.Modules;

public sealed class DockingModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<DockingManagerService>()
            .As<IDockingManagerService>()
            .SingleInstance();

        builder.RegisterType<DockableRegistry>()
            .As<IDockableRegistry>()
            .SingleInstance();

        builder.RegisterType<DockFactory>()
            .As<IDockFactory>()
            .SingleInstance();

        builder.RegisterType<DockLayoutPersistence>()
            .As<IDockLayoutPersistence>()
            .SingleInstance();

        // Dockables
        builder.RegisterType<LogDockable>()
            .As<IDockableDefinition>()
            .SingleInstance();

        builder.RegisterType<CellBrowserDockable>()
            .As<IDockableDefinition>()
            .SingleInstance();

        builder.RegisterType<AssetBrowserDockable>()
            .As<IDockableDefinition>()
            .SingleInstance();

        builder.RegisterType<ScriptEditorDockable>()
            .As<IDockableDefinition>()
            .SingleInstance();

        builder.RegisterType<RecordBrowserDockable>()
            .As<IDockableDefinition>()
            .SingleInstance();

        builder.RegisterType<ViewportDockable>()
            .As<IDockableDefinition>()
            .SingleInstance();
    }
}
