using System;
using System.IO;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CreationEditor.WPF.Modules;
using CreationEditor.WPF.Services.Docking;
using CreationEditor.WPF.Services.Startup;
using CreationEditor.WPF.Skyrim.Modules;
using CreationEditor.WPF.Views;
using Elscrux.Notification;
using Mutagen.Bethesda.Autofac;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.DI;
namespace CreationEditor.WPF.Skyrim;

public partial class App : Application {
    public App() {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnFirstChanceException;
    }
    
    private static void CurrentDomainOnFirstChanceException(object sender, UnhandledExceptionEventArgs e) {
        var exception = (Exception) e.ExceptionObject;
        
        using var log = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrashLog.txt"), false);
        log.WriteLine(exception);
    }
    
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var builder = new ContainerBuilder();
        
            builder.RegisterModule<MainModule>();
            builder.RegisterModule<NotificationModule>();
            builder.RegisterModule<Notification.NotificationModule>();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<MutagenModule>();
            builder.RegisterModule<SkyrimModule>();
        
            // ToDo
            // Remove
            builder.RegisterType<GameInstallModeContext>().AsImplementedInterfaces();
            builder.RegisterType<GameLocator>().As<IGameDirectoryLookup>();
        
            var window = new MainWindow();
            builder.RegisterInstance(window).As<IMainWindow>();

            var dockingManagerService = new DockingManagerService(window.DockPanel);
            builder.RegisterInstance(dockingManagerService).As<IDockingManagerService>();
        
            var container = builder.Build();
        
            container.Resolve<IStartup>()
                .Start();
            
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}