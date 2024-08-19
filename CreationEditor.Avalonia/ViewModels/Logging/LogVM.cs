using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Logging;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog.Events;
namespace CreationEditor.Avalonia.ViewModels.Logging;

public sealed class LogVM : ViewModel, ILogVM {
    public static readonly IReadOnlyList<LogEventLevel> LogLevels = Enum.GetValues<LogEventLevel>();

    public Dictionary<LogEventLevel, bool> LevelsVisibility { get; } = LogLevels.ToDictionary(x => x, _ => true);

    public int MaxLogCount { get; set; } = 1000;

    public IObservableCollection<ILogItem> LogItems { get; }

    public IObservableCollection<LogEventLevel> VisibilityLevels { get; } = new ObservableCollectionExtended<LogEventLevel>(LogLevels);

    public ReactiveCommand<Unit, Unit> Clear { get; }
    public ReactiveCommand<LogEventLevel, Unit> ToggleEvent { get; }
    public ReactiveCommand<Unit, Unit> ToggleAutoScroll { get; }

    [Reactive] public bool AutoScroll { get; set; } = true;

    private readonly SourceCache<ILogItem, Guid> _logAddedCache = new(item => item.Id);

    public LogVM(
        IObservableLogSink observableLogSink) {

        observableLogSink.LogAdded
            .Subscribe(log => _logAddedCache.AddOrUpdate(log))
            .DisposeWith(this);

        _logAddedCache.CountChanged
            .Subscribe(_ => {
                while (_logAddedCache.Count > MaxLogCount) {
                    var logItem = _logAddedCache.Items.MinBy(x => x.Time);
                    if (logItem is not null) _logAddedCache.Remove(logItem.Id);
                }
            })
            .DisposeWith(this);

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

        LogItems = _logAddedCache
            .Connect()
            .Filter(this.WhenAnyValue(x => x.VisibilityLevels.Count)
                .Select(_ => new Func<ILogItem, bool>(item => VisibilityLevels.Contains(item.Level))))
            .SortBy(item => item.Time)
            .ToObservableCollection(this);
    }
}
