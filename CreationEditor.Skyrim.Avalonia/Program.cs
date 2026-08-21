using System.IO.Abstractions;
using Autofac;
using Avalonia;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Avalonia.Services.Exceptions;
using CreationEditor.Services.Plugin;
using HotAvalonia;
using Noggog;
using ReactiveUI.Avalonia.Splat;
using ILogger = Serilog.ILogger;
namespace CreationEditor.Skyrim.Avalonia;

internal static class Program {
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args) {
        AppDomain.CurrentDomain.UnhandledException += LogCrashes;

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

        return BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static void LogCrashes(object sender, UnhandledExceptionEventArgs e) {
        var exception = (Exception) e.ExceptionObject;

        using var log = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrashLog.txt"), false);

        var innerException = exception.InnerException;
        while (innerException is not null) {
            if (innerException is DirectoryNotFoundException { Message: var message } && message.Contains("Data folder for SkyrimSE cannot be found automatically")) {
                const string note = "The Skyrim data folder could not be found automatically. Add the start argument --game-directory <path> or -g <path> to specify the game directory, or ensure that Skyrim is installed in a standard location.";
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
            .UseReactiveUIWithAutofac(builder => {
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
