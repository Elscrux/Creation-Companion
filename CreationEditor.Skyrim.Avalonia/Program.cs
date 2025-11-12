using System.IO.Abstractions;
using Avalonia;
using Avalonia.ReactiveUI;
using CreationEditor.Services.Plugin;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia;

internal static class Program {
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async Task Main(string[] args) {
        AppDomain.CurrentDomain.UnhandledException += LogCrashes;

        switch (args) {
            case [var verb, ..]:
                var pluginService = new PluginsFolderAssemblyProvider(new FileSystem());

                var entryPoint = pluginService.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.InheritsFrom(typeof(ICommandLineEntryPoint)))
                    .Select(Activator.CreateInstance)
                    .OfType<ICommandLineEntryPoint>()
                    .FirstOrDefault(entryPoint => entryPoint.Verbs.Contains(verb, StringComparer.OrdinalIgnoreCase));

                // If found run the entry point and exit
                if (entryPoint is not null) {
                    await entryPoint.Run(args);

                    Console.WriteLine("Done, press enter to exit");
                    Console.ReadLine();
                    return;
                }

                // Otherwise, if no entry point was found, log and continue to start the GUI
                Console.WriteLine($"Tried to start program with {verb}, but no plugin supports that verb");
                break;
        }

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static void LogCrashes(object sender, UnhandledExceptionEventArgs e) {
        var exception = (Exception) e.ExceptionObject;

        using var log = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrashLog.txt"), false);
        log.WriteLine(exception);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithDeveloperTools()
            .LogToTrace()
            .UseReactiveUI();
}
