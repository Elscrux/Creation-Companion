using System.Reactive.Subjects;
using CreationEditor.Avalonia.Models.Logging;
using Serilog.Events;
namespace CreationEditor.Avalonia.ViewModels.Logging; 

public class ReplayLogSink : IObservableLogSink {
    private readonly ReplaySubject<ILogItem> _logAdded = new();
    public IObservable<ILogItem> LogAdded => _logAdded;

    public void Emit(LogEvent logEvent) {
        var logMessage = logEvent.RenderMessage();
        AddText(logMessage, logEvent.Level);
    }

    private void AddText(string text, LogEventLevel level) {
        var logItem = new LogItem(text, level);
        
        _logAdded.OnNext(logItem);
    }
}
