using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor.WPF.Models;
using Elscrux.Logging.Sinks;
using ReactiveUI;
using Serilog.Events;
namespace CreationEditor.WPF.ViewModels;

public interface ILogVM : ILogListSink {
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }

    public static ISolidColorBrush VerboseBrush { get; } = Brushes.CornflowerBlue;
    public static ISolidColorBrush DebugBrush { get; } = Brushes.ForestGreen;
    public static ISolidColorBrush MessageBrush { get; } = Brushes.White;
    public static ISolidColorBrush WarningBrush { get; } = Brushes.Yellow;
    public static ISolidColorBrush ErrorBrush { get; } =   Brushes.Red;
    public static ISolidColorBrush FatalBrush { get; } =   Brushes.DarkRed;
}

public class LogVM : ReactiveObject, ILogVM {
    private const int MaxLogCount = 500;

    public ObservableCollection<ILogItem> LogItems { get; set; } = new();
    
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }

    public event ILogListSink.LogAddedHandler? OnLogAdded;

    public LogVM() {
        OnLogAdded += LimitLogCount;
        
        ClearCommand = ReactiveCommand.Create(Clear);
    }

    public void Emit(LogEvent logEvent) {
        var logMessage = logEvent.RenderMessage();
        AddText(logMessage, logEvent.Level);
    }

    private void AddText(string text, LogEventLevel level) {
        var logField = new LogItem(text, level);
        
        Dispatcher.UIThread.Post(() => LogItems.Add(logField));
        OnLogAdded?.Invoke(logField);
    }

    private void Clear() => LogItems.Clear();

    private void LimitLogCount(ILogItem logItem) {
        Dispatcher.UIThread.Post(() => {
            while (LogItems.Count > MaxLogCount) LogItems.RemoveAt(0);
        });
    }
}
