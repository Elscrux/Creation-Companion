using System.IO.Abstractions;
using Autofac;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Busy;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.Services.Startup;
using CreationEditor.Avalonia.Services.Viewport;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Background;
using CreationEditor.Services.Mutagen.References;
namespace CreationEditor.Avalonia.Modules; 

public sealed class MainModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<Lifecycle>()
            .As<ILifecycle>();
        
        builder.RegisterType<MainVM>()
            .SingleInstance();

        builder.RegisterInstance(new FileSystem())
            .As<IFileSystem>()
            .SingleInstance();
        
        builder.RegisterType<BackgroundTaskManager>()
            .As<IBackgroundTaskManager>()
            .SingleInstance();
        
        builder.RegisterType<BackgroundReferenceQuery>()
            .As<IReferenceQuery>()
            .SingleInstance();

        builder.RegisterType<BusyService>()
            .As<IBusyService>()
            .SingleInstance();

        builder.RegisterType<RecordBrowserSettingsVM>()
            .As<IRecordBrowserSettingsVM>();

        builder.RegisterType<RecordTypeProvider>()
            .AsSelf();

        builder.RegisterType<RecordListVM>()
            .As<IRecordListVM>();

        builder.RegisterType<RecordIdentifiersProvider>()
            .AsSelf();

        builder.RegisterType<RecordEditorController>()
            .As<IRecordEditorController>()
            .SingleInstance();

        builder.RegisterType<ExtraColumnProvider>()
            .As<IExtraColumnProvider>()
            .SingleInstance();

        builder.RegisterType<ExtraColumnsBuilder>()
            .As<IExtraColumnsBuilder>();

        builder.RegisterType<DockFactory>()
            .As<IDockFactory>()
            .SingleInstance();

        builder.RegisterType<BSEViewportFactory>()
            .As<IViewportFactory>();
        
        //VMs
        builder.RegisterType<ModSelectionVM>();
    }
}
