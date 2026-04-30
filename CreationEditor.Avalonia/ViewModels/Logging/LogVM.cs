using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Logging;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog.Events;
namespace CreationEditor.Avalonia.ViewModels.Logging;

public sealed partial class LogVM : ViewModel, ILogVM {
    public static readonly IReadOnlyList<LogEventLevel> LogLevels = Enum.GetValues<LogEventLevel>();

    public Dictionary<LogEventLevel, bool> LevelsVisibility { get; } = LogLevels.ToDictionary(x => x, _ => true);

    public int MaxLogCount { get; set; } = 1000;

    public IObservableCollection<ILogItem> LogItems { get; }

    public IObservableCollection<LogEventLevel> VisibilityLevels { get; } = new ObservableCollectionExtended<LogEventLevel>(LogLevels);

    [Reactive] public partial bool AutoScroll { get; set; }

    private readonly SourceCache<ILogItem, Guid> _logAddedCache = new(item => item.Id);

    public LogVM(IObservableLogSink observableLogSink) {
        AutoScroll = true;

        observableLogSink.LogAdded
            .Subscribe(AddLogItem)
            .DisposeWith(this);

        _logAddedCache.CountChanged
            .Subscribe(TrimLogItems)
            .DisposeWith(this);

        LogItems = _logAddedCache
            .Connect()
            .Filter(this.WhenAnyValue(x => x.VisibilityLevels.Count)
                .Select(_ => new Func<ILogItem, bool>(item => VisibilityLevels.Contains(item.Level))))
            .SortBy(item => item.Time)
            .ToObservableCollection(this);
    }

    [ReactiveCommand]
    private void ToggleAutoScroll() {
        AutoScroll = !AutoScroll;
    }

    [ReactiveCommand]
    private void ToggleEvent(LogEventLevel level) {
        LevelsVisibility.UpdateOrAdd(level,
            visibility => {
                if (visibility) {
                    VisibilityLevels.Remove(level);
                } else {
                    VisibilityLevels.Add(level);
                }
                return !visibility;
            });
    }

    [ReactiveCommand]
    private void Clear() {
        _logAddedCache.Clear();
    }

    private void AddLogItem(ILogItem log) {
        _logAddedCache.AddOrUpdate(log);
    }

    private void TrimLogItems() {
        while (_logAddedCache.Count > MaxLogCount) {
            var logItem = _logAddedCache.Items.MinBy(x => x.Time);
            if (logItem is not null) _logAddedCache.Remove(logItem.Id);
        }
    }
}
