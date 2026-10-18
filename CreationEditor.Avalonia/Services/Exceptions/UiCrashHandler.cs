using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Serilog;
namespace CreationEditor.Avalonia.Services.Exceptions;

/// <summary>
/// Simple UI crash handling: exceptions that reach the Avalonia dispatcher are logged, reported to the user,
/// and marked as handled so the UI keeps running.
///
/// Exceptions that leave the UI in a broken state are left unhandled, and need to be handled by an outer handler loop.
/// </summary>
public sealed class UiCrashHandler(ILogger logger) {
    /// <summary>
    /// Exception types that leave the UI in a broken state and require a UI restart.
    /// </summary>
    private static readonly HashSet<Type> FatalExceptionTypes = [
        typeof(XamlLoadException),
    ];

    public void Attach() {
        Dispatcher.UIThread.UnhandledException += OnUnhandledException;
    }

    private void OnUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e) {
        var exception = e.Exception;
        if (FatalExceptionTypes.Contains(exception.GetType())) {
            // Leave fatal exception unhandled
            logger.Here().Fatal(exception, "Fatal unhandled exception on the UI thread: {Message}", exception.Message);
            return;
        }

        logger.Here().Error(exception, "Unhandled exception on the UI thread: {Message}", exception.Message);

        e.Handled = true;
    }
}
