using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ReactiveUI;
namespace CreationEditor.GUI.Logging;

public class Log {
    public static ObservableCollection<LogItem> LogItems { get; set; } = new();
    private static int MaxLogCount { get; set; } = 500;
    public ICommand ClearCommand { get; }

    private const bool IncludeStackTrace = true;

    private static readonly Brush MessageBrush = Brushes.White;
    private static readonly Brush WarningBrush = Brushes.Yellow;
    private static readonly Brush ErrorBrush =   Brushes.Red;

    public delegate void LogAddedHandler(LogItem logItem);
    public static event LogAddedHandler? OnLogAdded;

    public Log() {
        OnLogAdded += LimitLogCount;

        ClearCommand = ReactiveCommand.Create(Clear);
    }

    public static void Message(string text, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = -1) => AddText(text, MessageBrush, callerName, callerPath, callerLine);
    public static void Warning(string text, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = -1) => AddText(text, WarningBrush, callerName, callerPath, callerLine);
    public static void Error  (string text, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = -1) => AddText(text, ErrorBrush,   callerName, callerPath, callerLine);

    public static void Clear() => LogItems.Clear();

    private static void AddText(string text, Brush color, string callerName = "", string callerPath = "", int callerLine = -1) {
        if (IncludeStackTrace) {
            text = $"{Path.GetFileName(callerPath)}: Line {callerLine} - {callerName}: {text}";
        }
        var logField = new LogItem(text, color);
        
        Application.Current.Dispatcher.Invoke(() => LogItems.Add(logField));
        OnLogAdded?.Invoke(logField);
    }

    private static void LimitLogCount(LogItem logItem) {
        Dispatcher.CurrentDispatcher.Invoke(() => {
            while (LogItems.Count > MaxLogCount) LogItems.RemoveAt(0);
        });
    }
}
