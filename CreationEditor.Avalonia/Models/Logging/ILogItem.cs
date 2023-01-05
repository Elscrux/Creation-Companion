using Serilog.Events;
namespace CreationEditor.Avalonia.Models.Logging;

public interface ILogItem {
    Guid Id { get; set; }
    string Text { get; set; }
    LogEventLevel Level { get; set; }
}
