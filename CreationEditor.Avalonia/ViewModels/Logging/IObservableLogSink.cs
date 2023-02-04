using CreationEditor.Avalonia.Models.Logging;
using Serilog.Core;
namespace CreationEditor.Avalonia.ViewModels.Logging; 

public interface IObservableLogSink : ILogEventSink {
    public IObservable<ILogItem> LogAdded { get; }
}