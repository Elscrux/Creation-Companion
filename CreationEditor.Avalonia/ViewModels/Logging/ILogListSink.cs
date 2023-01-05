using CreationEditor.Avalonia.Models.Logging;
using Serilog.Core;
namespace CreationEditor.Avalonia.ViewModels.Logging; 

public interface ILogListSink : ILogEventSink {
    public IObservable<ILogItem> LogAdded { get; }
}