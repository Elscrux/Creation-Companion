using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Subjects;
using CreationEditor.Avalonia.Models.Logging;
using CreationEditor.Extension;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog.Events;
using ILogItem = CreationEditor.Avalonia.Models.Logging.ILogItem;
namespace CreationEditor.Avalonia.ViewModels.Logging;

public sealed class LogVM : ViewModel, ILogVM {
    public static readonly LogEventLevel[] LogLevels = Enum.GetValues<LogEventLevel>();
    
    public Dictionary<LogEventLevel, bool> LevelsVisibility { get; } = LogLevels.ToDictionary(x => x, _ => true);
    
    public int MaxLogCount { get; set; } = 500;
    
    public IObservableCollection<ILogItem> LogItems { get; }
    
    public ObservableCollection<LogEventLevel> VisibilityLevels { get; } = new(LogLevels);
    
    public ReactiveCommand<Unit, Unit> Clear { get; }
    public ReactiveCommand<LogEventLevel, Unit> ToggleEvent { get; }
    public ReactiveCommand<Unit, Unit> ToggleAutoScroll { get; }

    [Reactive] public bool AutoScroll { get; set; } = true;

    private readonly SourceCache<ILogItem, Guid> _logAddedCache = new(item => item.Id);
    
    private readonly Subject<ILogItem> _logAdded = new();
    public IObservable<ILogItem> LogAdded => _logAdded;

   
    public LogVM() {
        Clear = ReactiveCommand.Create<Unit>(_ => _logAddedCache.Clear());

        ToggleEvent = ReactiveCommand.Create<LogEventLevel>(level => {
            LevelsVisibility.UpdateOrAdd(level, visibility => {
                if (visibility) {
                    VisibilityLevels.Remove(level);
                } else {
                    VisibilityLevels.Add(level);
                }
                return !visibility;
            });
        });

        ToggleAutoScroll = ReactiveCommand.Create(() => {
            AutoScroll = !AutoScroll;
        });
        
        this.WhenAnyValue(x => x.VisibilityLevels.Count)
            .Subscribe(_ => _logAddedCache.Refresh())
            .DisposeWith(this);

        LogItems = _logAddedCache
            .Connect()
            .Filter(item => VisibilityLevels.Contains(item.Level))
            .LimitSizeTo(MaxLogCount)
            .SortBy(item => item.Time)
            .ToObservableCollection(this);
    }

    public void Emit(LogEvent logEvent) {
        var logMessage = logEvent.RenderMessage();
        AddText(logMessage, logEvent.Level);
    }

    private void AddText(string text, LogEventLevel level) {
        var logItem = new LogItem(text, level);
        
        _logAdded.OnNext(logItem);
        _logAddedCache.AddOrUpdate(logItem);
    }
}
