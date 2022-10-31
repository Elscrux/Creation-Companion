using System;
using System.IO;
using System.Windows;
using Autofac;
using CreationEditor.WPF.Modules;
using CreationEditor.WPF.Services.Docking;
using CreationEditor.WPF.Services.Startup;
using CreationEditor.WPF.Skyrim.Modules;
using CreationEditor.WPF.Views;
using Elscrux.Notification;
namespace CreationEditor.WPF.Skyrim;

public partial class App {
    public App() {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnFirstChanceException;
    }
    
    private static void CurrentDomainOnFirstChanceException(object sender, UnhandledExceptionEventArgs e) {
        var exception = (Exception) e.ExceptionObject;
        
        using var log = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrashLog.txt"), false);
        log.WriteLine(exception);
    }

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        
        var builder = new ContainerBuilder();
        
        builder.RegisterModule<MainModule>();
        builder.RegisterModule<NotificationModule>();
        builder.RegisterModule<LoggingModule>();
        // builder.RegisterModule<MutagenModule>();
        builder.RegisterModule<SkyrimModule>();
        
        var window = new MainWindow();
        builder.RegisterInstance(window).As<IMainWindow>();

        var dockingManagerService = new DockingManagerService(window.DockingManager);
        builder.RegisterInstance(dockingManagerService).As<IDockingManagerService>();
        
        var container = builder.Build();
        
        container.Resolve<IStartup>()
            .Start();

        window.Show();
    }
}