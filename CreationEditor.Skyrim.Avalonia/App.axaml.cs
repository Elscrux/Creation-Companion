using System;
using System.IO;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.Services.Exceptions;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Skyrim.Avalonia.Modules;
using Mutagen.Bethesda.Autofac;
using ReactiveUI;
using Serilog;
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

            var window = new MainWindow();
            builder.RegisterInstance(window).As<MainWindow>();

            var dockingManagerService = new DockingManagerService();
            builder.RegisterInstance(dockingManagerService).As<IDockingManagerService>();

            var container = builder.Build();

            // Init ReactiveUI
            RxApp.DefaultExceptionHandler = new ObservableExceptionHandler(container.Resolve<ILogger>());

            // Handle lifecycle
            var lifecycle = container.Resolve<ILifecycle>();
            lifecycle.Start();

            desktop.Exit += (_, _) => lifecycle.Exit();

            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
