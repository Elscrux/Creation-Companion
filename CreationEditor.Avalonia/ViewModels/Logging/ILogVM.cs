using System.Reactive;
using CreationEditor.Avalonia.Models.Logging;
using DynamicData.Binding;
using ReactiveUI;
using Serilog.Events;
namespace CreationEditor.Avalonia.ViewModels.Logging;

public interface ILogVM : ILogListSink {
    public IObservableCollection<ILogItem> VisibleLogItems { get; set; }
    public Dictionary<LogEventLevel, bool> LevelsVisibility { get; }

    public ReactiveCommand<Unit, Unit> Clear { get; }
    public ReactiveCommand<LogEventLevel, Unit> ToggleEvent { get; }
}