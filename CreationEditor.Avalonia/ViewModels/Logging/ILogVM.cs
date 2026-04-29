using System.Reactive;
using CreationEditor.Avalonia.Models.Logging;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using Serilog.Events;
namespace CreationEditor.Avalonia.ViewModels.Logging;

public interface ILogVM : IDisposableDropoff {
    IObservableCollection<ILogItem> LogItems { get; }
    Dictionary<LogEventLevel, bool> LevelsVisibility { get; }

    ReactiveCommand<Unit, Unit> ClearCommand { get; }
    ReactiveCommand<LogEventLevel, Unit> ToggleEventCommand { get; }
    ReactiveCommand<Unit, Unit> ToggleAutoScrollCommand { get; }

    bool AutoScroll { get; set; }
}
