using Serilog.Events;
namespace CreationEditor.Avalonia.Models.Logging;

public sealed record LogItem(string Text, LogEventLevel Level) : ILogItem {
    public Guid Id { get; set; } = Guid.NewGuid();
    public LogEventLevel Level { get; set; } = Level;
    public string Text { get; set; } = Text;
}
