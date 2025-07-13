using System.IO.Abstractions;
using System.Reactive.Subjects;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.Services.Exceptions;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Plugin;
using CreationEditor.Skyrim.Avalonia.Modules;
using HotAvalonia;
using Mutagen.Bethesda.Autofac;
using Noggog;
using ReactiveUI;
using Serilog;
namespace CreationEditor.Skyrim.Avalonia;

public class App : Application {
    public App() {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnFirstChanceException;
    }

    private static void CurrentDomainOnFirstChanceException(object sender, UnhandledExceptionEventArgs e) {
        var exception = (Exception) e.ExceptionObject;

        using var log = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrashLog.txt"), false);
        log.WriteLine(exception);
    }

    public override void Initialize() {
        this.EnableHotReload();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var builder = new ContainerBuilder();

            builder.RegisterModule<MutagenModule>();
            builder.RegisterModule<EditorModule>();
            builder.RegisterModule<SkyrimModule>();

            LoadPluginModules(builder);

            var window = new MainWindow();
            builder.RegisterInstance(window).As<MainWindow>();

            var dockingManagerService = new DockingManagerService();
            builder.RegisterInstance(dockingManagerService).As<IDockingManagerService>();

            var container = builder.Build();

            // Init ReactiveUI
            RxApp.DefaultExceptionHandler = new ObservableExceptionHandler(container.Resolve<ILogger>());

            // Do startup
            var lifecycleStartup = container.Resolve<ILifecycleStartup>();
            var logger = container.Resolve<ILogger>();
            logger.Here().Information("Starting the application");
            lifecycleStartup.PreStartup();

            logger.Here().Information("Initializing the UI");
            window.ViewModel = container.Resolve<MainVM>();
            desktop.MainWindow = window;

            desktop.Exit += (_, _) => lifecycleStartup.Exit();

            Task.Run(() => lifecycleStartup.PostStartupAsync(new Subject<string>()));
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void LoadPluginModules(ContainerBuilder builder) {
        // Load plugins extension modules to include in the base container
        // TODO: Maybe show a confirmation dialog to select which plugins to load?
        var pluginService = new PluginsFolderAssemblyProvider(new FileSystem());

        // Note that loading the assemblies alone can lead to
        // additional registrations being made through assembly scanning
        var extensionModules = pluginService.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.InheritsFrom(typeof(ExtensionModule)))
            .Select(Activator.CreateInstance)
            .OfType<ExtensionModule>()
            .ToList();

        foreach (var extensionModule in extensionModules) {
            builder.RegisterModule(extensionModule);
        }
    }
}
