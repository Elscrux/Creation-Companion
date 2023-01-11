using Serilog.Events;
namespace CreationEditor.Avalonia.Models.Logging;

public sealed record LogItem(string Text, LogEventLevel Level) : ILogItem {
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime Time { get; } = DateTime.Now;
}
