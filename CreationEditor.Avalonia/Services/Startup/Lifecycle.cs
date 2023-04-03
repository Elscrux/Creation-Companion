using Autofac;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.Views;
using Serilog;
namespace CreationEditor.Avalonia.Services.Startup;

public sealed class Lifecycle : ILifecycle {
    private readonly ILogger _logger;
    private readonly MainWindow _mainWindow;
    private readonly Lazy<MainVM> _mainVm;

    private readonly List<ILifecycleTask> _lifecycleTasks;

    public Lifecycle(
        IComponentContext componentContext,
        ILogger logger,
        MainWindow mainWindow,
        Lazy<MainVM> mainVm) {
        _logger = logger;
        _mainWindow = mainWindow;
        _mainVm = mainVm;

        _lifecycleTasks = typeof(ILifecycleTask)
            .GetAllSubClass<ILifecycleTask>(componentContext.Resolve)
            .ToList();
    }

    public void Start() {
        _logger.Here().Information("Starting the application");

        // Handle lifecycle tasks
        _logger.Here().Debug("Run {Count} Lifecycle Task(s) on Startup", _lifecycleTasks.Count);
        _lifecycleTasks.ForEach(task => task.OnStartup());

        _mainWindow.DataContext = _mainVm.Value;
    }

    public void Exit() {
        // Handle lifecycle tasks
        _logger.Here().Debug("Run {Count} Lifecycle Task(s) on Exit", _lifecycleTasks.Count);
        _lifecycleTasks.ForEach(task => task.OnExit());
    }
}
