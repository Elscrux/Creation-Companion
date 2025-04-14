using CreationEditor.Services.DataSource;
namespace CreationEditor.Services.Asset;

public interface IDataSourceWatcherProvider {
    /// <summary>
    /// Returns a watcher for the given data source.
    /// </summary>
    /// <param name="dataSource">Data source to create a watcher for</param>
    /// <returns>Watcher for the data source</returns>
    IDataSourceWatcher GetWatcher(IDataSource dataSource);
}
