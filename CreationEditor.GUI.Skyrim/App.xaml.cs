using System;
using System.IO;
using System.Windows;
using Autofac;
using CreationEditor.GUI.Modules;
using CreationEditor.GUI.Services.Docking;
using CreationEditor.GUI.Services.Startup;
using CreationEditor.GUI.Skyrim.Modules;
using CreationEditor.GUI.Views;
using Elscrux.Notification;
using Syncfusion.Windows.Tools.Controls;
namespace CreationEditor.GUI.Skyrim;

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

        var dockingManagerService = new DockingManagerService(DockingManager.GetDockingManager(window));
        builder.RegisterInstance(dockingManagerService).As<IDockingManagerService>();
        
        var container = builder.Build();
        
        container.Resolve<IStartup>()
            .Start();

        window.Show();
    }
}