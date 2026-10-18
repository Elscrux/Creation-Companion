using System.IO.Abstractions;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommandLine;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.Services.Exceptions;
using CreationEditor.Avalonia.Services.Host;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Notification;
using CreationEditor.Services.Plugin;
using CreationEditor.Skyrim.Avalonia.Modules;
using CreationEditor.Skyrim.Avalonia.Options;
using HotAvalonia;
using Noggog;
using ReactiveUI.Avalonia.Splat;
using Serilog;
namespace CreationEditor.Skyrim.Avalonia;

internal static class Program {
    public static EditorHost Host { get; } = new();

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args) {
        AppDomain.CurrentDomain.UnhandledException += LogCrashes;
        TaskScheduler.UnobservedTaskException += LogUnobservedTaskException;

        switch (args) {
            case [var verb, ..] when !verb.StartsWith('-'):
                var pluginService = new PluginsFolderAssemblyProvider(new FileSystem());

                var entryPoint = pluginService.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.InheritsFrom(typeof(ICommandLineEntryPoint)))
                    .Select(Activator.CreateInstance)
                    .OfType<ICommandLineEntryPoint>()
                    .FirstOrDefault(entryPoint => entryPoint.Verbs.Contains(verb, StringComparer.OrdinalIgnoreCase));

                // If found run the entry point and exit
                if (entryPoint is not null) {
                    entryPoint.Run(args).GetAwaiter().GetResult();

                    Console.WriteLine("Done, press enter to exit");
                    Console.ReadLine();
                    return 0;
                }

                // Otherwise, if no entry point was found, log and continue to start the GUI
                Console.WriteLine($"Tried to start program with {verb}, but no plugin supports that verb");
                break;
        }

        var globalOptionsParsed = Parser.Default.ParseArguments<GlobalOptions>(args);

        // Build the host (DI container + lifecycle) before the UI starts, so the core is
        // independent of the UI lifetime
        Host.Build(builder => {
            builder.RegisterModule<SkyrimModule>();

            if (globalOptionsParsed.Value?.GameDirectory is not null) {
                builder.Register<InjectedGameDirectoryLookup>(x => {
                        var fileSystem = x.Resolve<IFileSystem>();
                        return new InjectedGameDirectoryLookup(fileSystem, globalOptionsParsed.Value.GameDirectory);
                    })
                    .SingleInstance()
                    .AsImplementedInterfaces();
            }
        });

        ClassicDesktopStyleApplicationLifetime? lifetime = null;
        BuildAvaloniaApp()
            .SetupWithClassicDesktopLifetime(args,
                l => {
                    lifetime = (ClassicDesktopStyleApplicationLifetime) l;
                });

        Host.PreStartup();

        var container = Host.Container;
        StartUi(lifetime!, container);
        Host.PostStartup();

        var recoveries = new List<DateTimeOffset>();
        try {
            while (true) {
                try {
                    lifetime!.Start(args);
                    break; // Normal exit
                } catch (Exception ex) {
                    Log.Logger.Here().Error(ex, "UI crashed, restarting the UI");

                    var now = DateTimeOffset.UtcNow;
                    recoveries.RemoveAll(time => now - time > TimeSpan.FromSeconds(30));
                    recoveries.Add(now);
                    if (recoveries.Count > 3) {
                        Log.Logger.Here().Error(ex, "UI keeps crashing, giving up after {Count} recoveries in 30 seconds", recoveries.Count - 1);
                        EmergencySave();
                        return 1;
                    }

                    RestartUi(lifetime!);
                }
            }
            return 0;
        } finally {
            Host.Exit();
        }
    }

    private static void RestartUi(ClassicDesktopStyleApplicationLifetime lifetime) {
        var container = Host.Container;

        var oldWindow = lifetime.MainWindow;
        var mainWindow = StartUi(lifetime, container);
        oldWindow?.Close();
        mainWindow.Show();

        try {
            var notificationService = container.Resolve<INotificationService>();
            var linearNotifier = new LinearNotifier(notificationService);
            linearNotifier.Next("The UI restarted after an error. Your session was preserved.");
            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => linearNotifier.Stop());
        } catch (Exception ex) {
            Log.Logger.Here().Error(ex, "Failed to show UI restart notification");
        }
    }

    private static MainWindow StartUi(ClassicDesktopStyleApplicationLifetime desktop, ILifetimeScope container) {
        var mainWindowProvider = container.Resolve<IMainWindowProvider>();

        // Detach docked controls from the previous window's visual tree so they can be
        // hosted in the new window; the docking state itself is preserved
        container.Resolve<IDockingManagerService>().DetachControls();

        var window = new MainWindow();
        mainWindowProvider.SetMainWindow(window);
        window.ViewModel = container.Resolve<MainVM>();
        desktop.MainWindow = window;

        // Show immediately: the window only registers with the lifetime on Show, and the old
        // window must not be closed while no other window is registered (that would shut the
        // dispatcher down via ShutdownMode.OnLastWindowClose)
        window.Show();

        return window;
    }

    private static void EmergencySave() {
        try {
            var container = Host.Container;
            var editorEnvironment = container.Resolve<IEditorEnvironment>();
            var modSaveService = container.Resolve<IModSaveService>();
            modSaveService.SaveMod(editorEnvironment.ActiveMod);
            Log.Logger.Here().Information("Emergency saved active mod {Mod}", editorEnvironment.ActiveMod.ModKey);
        } catch (Exception ex) {
            Log.Logger.Here().Error(ex, "Emergency save failed");
        }
    }

    private static void LogUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) {
        e.SetObserved();
        try {
            Log.Logger.Here().Error(e.Exception, "Unobserved task exception");
        } catch {
            Console.WriteLine($"Unobserved task exception: {e.Exception}");
        }
    }

    private static void LogCrashes(object sender, UnhandledExceptionEventArgs e) {
        var exception = (Exception) e.ExceptionObject;

        using var log = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrashLog.txt"), false);

        var innerException = exception.InnerException;
        while (innerException is not null) {
            if (innerException is DirectoryNotFoundException { Message: var message } && message.Contains("Data folder for SkyrimSE cannot be found automatically")) {
                const string note =
                    "The Skyrim data folder could not be found automatically. Add the start argument --game-directory <path> or -g <path> to specify the game directory, or ensure that Skyrim is installed in a standard location.";
                log.WriteLine(note);

                Console.WriteLine(note);
                Console.ReadKey();

                break;
            }

            innerException = innerException.InnerException;
        }

        log.WriteLine(exception);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UseHotReload()
            .UsePlatformDetect()
            .WithDeveloperTools()
            .LogToTrace()
            .UseReactiveUIWithAutofac(_ => {
                    // Register dependencies in the container
                },
                withReactiveUIBuilder: rxBuilder => {
                    var builder = new ContainerBuilder();
                    builder.RegisterInstance(new FileSystem()).As<IFileSystem>().SingleInstance();
                    builder.RegisterModule<LoggingModule>();
                    var container = builder.Build();
                    var logger = container.Resolve<ILogger>();
                    rxBuilder.WithExceptionHandler(new ObservableExceptionHandler(logger));
                });
}
