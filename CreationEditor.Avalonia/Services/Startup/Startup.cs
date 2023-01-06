using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.Views;
using Elscrux.Logging;
using Elscrux.Notification;
using Serilog;
namespace CreationEditor.Avalonia.Services.Startup;

public sealed class Startup : IStartup {
    private readonly INotifier _notifier;
    private readonly ILogger _logger;
    private readonly IMainWindow _mainWindow;
    private readonly Lazy<MainVM> _mainVm;

    public Startup(
        INotifier notifier, 
        ILogger logger,
        IMainWindow mainWindow,
        Lazy<MainVM> mainVm) {
        _notifier = notifier;
        _logger = logger;
        _mainWindow = mainWindow;
        _mainVm = mainVm;
    }

    public void Start() {
        _logger.Here().Information("Starting the application");
        
        _mainWindow.DataContext = _mainVm.Value;
    }
}
