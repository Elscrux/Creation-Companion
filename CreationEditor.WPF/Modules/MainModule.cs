using System.IO.Abstractions;
using Autofac;
using CreationEditor.Environment;
using CreationEditor.WPF.Models.Record.Browser;
using CreationEditor.WPF.Services;
using CreationEditor.WPF.Services.Startup;
using CreationEditor.WPF.ViewModels;
using CreationEditor.WPF.ViewModels.Mod;
using CreationEditor.WPF.ViewModels.Record;
using MutagenLibrary.References.ReferenceCache;
namespace CreationEditor.WPF.Modules; 

public class MainModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<Startup>()
            .As<IStartup>();
        
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

        builder.RegisterType<RecordBrowserSettings>()
            .As<IRecordBrowserSettings>();

        builder.RegisterType<RecordListVMReadOnly>()
            .AsSelf();
        
        //VMs
        builder.RegisterType<ModSelectionVM>();
    }
}
