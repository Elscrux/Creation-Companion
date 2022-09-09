using System;
using CreationEditor.GUI.ViewModels;
using CreationEditor.GUI.Views.Windows;
using Elscrux.Logging;
using Elscrux.Notification;
using Serilog;
namespace CreationEditor.GUI.Services.Startup;

public interface IStartup {
    void Start();
}

public class Startup : IStartup {
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
