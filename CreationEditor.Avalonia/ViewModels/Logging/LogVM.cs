using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CreationEditor.Avalonia.Models.Logging;
using CreationEditor.Extension;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using Serilog.Events;
using ILogItem = CreationEditor.Avalonia.Models.Logging.ILogItem;
namespace CreationEditor.Avalonia.ViewModels.Logging;

public sealed class LogVM : ViewModel, ILogVM {
    public static readonly LogEventLevel[] LogLevels = Enum.GetValues<LogEventLevel>();
    
    public Dictionary<LogEventLevel, bool> LevelsVisibility { get; } = LogLevels.ToDictionary(x => x, _ => true);
    
    public uint MaxLogCount { get; set; } = 500;
    
    public ObservableCollection<ILogItem> LogItems { get; } = new();
    public IObservableCollection<ILogItem> VisibleLogItems { get; set; }
    
    public ObservableCollection<LogEventLevel> VisibilityLevels { get; } = new(LogLevels);
    
    public ReactiveCommand<Unit, Unit> Clear { get; }
    public ReactiveCommand<LogEventLevel, Unit> ToggleEvent { get; }
    
    private readonly Subject<ILogItem> _logAdded = new();
    public IObservable<ILogItem> LogAdded => _logAdded;

    public LogVM() {
        Clear = ReactiveCommand.Create<Unit>(_ => LogItems.Clear());

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

        LogAdded.Subscribe(_ => LimitLogCount());

        this.WhenAnyValue(x => x.MaxLogCount)
            .Subscribe(_ => LimitLogCount());
        
        VisibleLogItems = this.WhenAnyValue(x => x.VisibilityLevels.Count, x => x.LogItems.Count)
            .ObserveOnTaskpool()
            .Select(_ => {
                return Observable.Create<ILogItem>((obs, cancel) => {
                    foreach (var logItem in LogItems) {
                        if (cancel.IsCancellationRequested) return Task.CompletedTask;

                        if (!VisibilityLevels.Contains(logItem.Level)) continue;

                        obs.OnNext(logItem);
                    }
                    obs.OnCompleted();
                    return Task.CompletedTask;
                }).ToObservableChangeSet(x => x.Id);
            })
            .Switch()
            .ToObservableCollection(this);
    }

    public void Emit(LogEvent logEvent) {
        var logMessage = logEvent.RenderMessage();
        AddText(logMessage, logEvent.Level);
    }

    private void AddText(string text, LogEventLevel level) {
        var logItem = new LogItem(text, level);
        
        LogItems.Add(logItem);
        _logAdded.OnNext(logItem);
    }

    private void LimitLogCount() {
        while (LogItems.Count > MaxLogCount) LogItems.RemoveAt(0);
    }
}
