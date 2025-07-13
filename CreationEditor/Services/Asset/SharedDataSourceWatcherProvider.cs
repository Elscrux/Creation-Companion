using CreationEditor.Services.DataSource;
using Noggog;
namespace CreationEditor.Services.Asset;

public sealed class SharedDataSourceWatcherProvider : IDataSourceWatcherProvider, IDisposable {
    private readonly Lock _lock = new();
    private readonly DisposableBucket _disposables = new();

    private readonly List<IDataSourceWatcher> _dataSourceWatchers = [];
    private readonly Func<IDataSource, IDataSourceWatcher> _dataSourceWatcherFactory;

    public SharedDataSourceWatcherProvider(
        IDataSourceService dataSourceService,
        Func<IDataSource, IDataSourceWatcher> dataSourceWatcherFactory) {
        _dataSourceWatcherFactory = dataSourceWatcherFactory;

        dataSourceService.DataSourcesChanged
            .Subscribe(dataSources => {
                lock (_lock) {
                    var invalidWatchers = _dataSourceWatchers
                        .Where(watcher => !dataSources.Contains(watcher.DataSource))
                        .ToArray();

                    foreach (var watcher in invalidWatchers) {
                        watcher.Dispose();
                        _dataSourceWatchers.Remove(watcher);
                    }
                }
            })
            .DisposeWith(_disposables);
    }


    public IDataSourceWatcher GetWatcher(IDataSource dataSource) {
        lock (_lock) {
            var watcher = _dataSourceWatchers.FirstOrDefault(w => Equals(w.DataSource, dataSource));
            if (watcher is not null) return watcher;

            watcher = _dataSourceWatcherFactory(dataSource);
            _dataSourceWatchers.Add(watcher);

            return watcher;
        }
    }

    public void Dispose() {
        _disposables.Dispose();

        foreach (var watcher in _dataSourceWatchers) {
            watcher.Dispose();
        }

        _dataSourceWatchers.Clear();
    }
}
