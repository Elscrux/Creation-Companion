using System.IO.Abstractions;
using System.Reactive.Subjects;
using Autofac;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Plugin;
using Mutagen.Bethesda.Autofac;
using Serilog;
namespace CreationEditor.Avalonia.Services.Host;

/// <summary>
/// Owns the application's DI container and lifecycle.
/// Is detached from the Avalonia UI and built once per process.
/// This handles restarting the UI after a crash while keeping the editor state alive.
/// </summary>
public sealed class EditorHost {
    private ILifetimeScope? _container;
    private ILifecycleStartup? _lifecycleStartup;
    private bool _postStartupStarted;

    public ILifetimeScope Container => _container ?? throw new InvalidOperationException("EditorHost has not been built yet.");

    /// <summary>
    /// Builds the DI container.
    /// </summary>
    /// <param name="configure">An optional action to configure the container builder with game-specific modules and options.</param>
    public void Build(Action<ContainerBuilder>? configure = null) {
        var builder = new ContainerBuilder();

        builder.RegisterModule<MutagenModule>();
        builder.RegisterModule<EditorModule>();

        configure?.Invoke(builder);

        LoadPluginModules(builder);

        _container = builder.Build();
    }

    public void PreStartup() {
        var logger = Container.Resolve<ILogger>();
        logger.Here().Information("Starting the application");

        _lifecycleStartup = Container.Resolve<ILifecycleStartup>();
        _lifecycleStartup.PreStartup();
    }

    public void PostStartup() {
        if (_postStartupStarted) return;

        _postStartupStarted = true;

        Task.Run(() => _lifecycleStartup!.PostStartupAsync(new Subject<string>()));
    }

    public void Exit() {
        if (_lifecycleStartup is null) return;

        var logger = Container.Resolve<ILogger>();
        logger.Here().Information("Exiting the application");
        _lifecycleStartup.Exit();
    }

    private static void LoadPluginModules(ContainerBuilder builder) {
        // Load plugins extension modules to include in the base container
        var pluginService = new PluginsFolderAssemblyProvider(new FileSystem());

        // Note that loading the assemblies alone can lead to
        // additional registrations being made through assembly scanning
        var extensionModules = pluginService.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(ExtensionModule).IsAssignableFrom(type) && type is { IsAbstract: false, IsInterface: false })
            .Select(Activator.CreateInstance)
            .OfType<ExtensionModule>()
            .ToList();

        foreach (var extensionModule in extensionModules) {
            builder.RegisterModule(extensionModule);
        }
    }
}
