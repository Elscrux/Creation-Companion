using System.Reactive.Subjects;
using CreationEditor.Avalonia.Views;
namespace CreationEditor.Avalonia.Services.Avalonia;

/// <summary>
/// Provides access to the current main window without holding a direct reference to it.
/// The main window instance can be replaced at runtime (e.g. when the UI is restarted after a crash),
/// so services must resolve the window through this provider at call time instead of caching it.
/// </summary>
public interface IMainWindowProvider {
    /// <summary>Gets the current main window, or null if none has been created yet.</summary>
    MainWindow? Current { get; }

    /// <summary>An observable that emits the new main window instance whenever it changes.</summary>
    IObservable<MainWindow> MainWindowChanged { get; }

    /// <summary>Sets the current main window and notifies subscribers.</summary>
    void SetMainWindow(MainWindow? window);
}

public sealed class MainWindowProvider : IMainWindowProvider {
    public MainWindow? Current { get; private set; }

    private readonly Subject<MainWindow> _mainWindowChanged = new();
    public IObservable<MainWindow> MainWindowChanged => _mainWindowChanged;

    public void SetMainWindow(MainWindow? window) {
        Current = window;
        if (window is not null) {
            _mainWindowChanged.OnNext(window);
        }
    }
}
