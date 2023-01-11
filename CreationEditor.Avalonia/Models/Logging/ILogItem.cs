using Serilog.Events;
namespace CreationEditor.Avalonia.Models.Logging;

public interface ILogItem {
    Guid Id { get; }
    public DateTime Time { get; }
    
    string Text { get; init; }
    LogEventLevel Level { get; init; }
}
