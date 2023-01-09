using System;
using System.IO;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.Services.Startup;
using CreationEditor.Avalonia.Views;
using CreationEditor.Skyrim.Avalonia.Modules;
using Mutagen.Bethesda.Autofac;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.DI;
using NotificationModule = CreationEditor.Avalonia.Modules.NotificationModule;
namespace CreationEditor.Skyrim.Avalonia;

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
            builder.RegisterModule<NotificationModule>();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<MutagenModule>();
            builder.RegisterModule<SkyrimModule>();
        
            // Remove
            builder.RegisterType<GameLocator>().As<IGameDirectoryLookup>();
        
            var window = new MainWindow();
            builder.RegisterInstance(window).As<IMainWindow>();

            var dockingManagerService = new DockingManagerService();
            // var dockingManagerService = new DockingManagerService();
            // var dockingManagerService = new DockingManagerService(window.DockingManager);
            builder.RegisterInstance(dockingManagerService).As<IDockingManagerService>();
        
            var container = builder.Build();

            var lifecycle = container.Resolve<ILifecycle>();
            lifecycle.Start();

            desktop.Exit += (_, _) => lifecycle.Exit();
            
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}