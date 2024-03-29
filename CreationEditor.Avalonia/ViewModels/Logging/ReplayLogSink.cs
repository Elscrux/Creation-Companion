﻿using System.Reactive.Subjects;
using CreationEditor.Avalonia.Models.Logging;
using Serilog.Events;
namespace CreationEditor.Avalonia.ViewModels.Logging;

public sealed class ReplayLogSink : IObservableLogSink {
    private const int ReplayLimit = 500;

    private readonly ReplaySubject<ILogItem> _logAdded = new(ReplayLimit);
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
