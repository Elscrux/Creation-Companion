using Avalonia.Media;
using CreationEditor.WPF.ViewModels;
using Elscrux.Logging.Sinks;
using Serilog.Events;
namespace CreationEditor.WPF.Models;

public record LogItem(string Text, LogEventLevel Level) : ILogItem {
    public LogEventLevel Level { get; set; } = Level;
    public string Text { get; set; } = Text;
    
    public ISolidColorBrush Color => Level switch {
        LogEventLevel.Verbose => ILogVM.VerboseBrush,
        LogEventLevel.Debug => ILogVM.DebugBrush,
        LogEventLevel.Information => ILogVM.MessageBrush,
        LogEventLevel.Warning => ILogVM.WarningBrush,
        LogEventLevel.Error => ILogVM.ErrorBrush,
        LogEventLevel.Fatal => ILogVM.FatalBrush,
        _ => throw new ArgumentOutOfRangeException()
    };
}
