using CreationEditor.Services.DataSource;
namespace CreationEditor.Services.Asset;

public interface IDataSourceWatcher : IDisposable {
    /// <summary>
    /// The data source that this watcher is watching.
    /// </summary>
    IDataSource DataSource { get; }

    /// <summary>
    /// Emits the event args of a newly created file or directory in the data source.
    /// </summary>
    IObservable<DataSourceLink> Created { get; }
    /// <summary>
    /// Emits the event args of a newly created file in the data source.
    /// </summary>
    IObservable<DataSourceLink> CreatedFile { get; }
    /// <summary>
    /// Emits the event args of a newly created directory in the data source.
    /// </summary>
    IObservable<DataSourceLink> CreatedDirectory { get; }

    /// <summary>
    /// Emits the event args of a deleted file or directory in the data source.
    /// </summary>
    IObservable<DataSourceLink> Deleted { get; }
    /// <summary>
    /// Emits the event args of a deleted file in the data source.
    /// </summary>
    IObservable<DataSourceLink> DeletedFile { get; }
    /// <summary>
    /// Emits the event args of a deleted directory in the data source.
    /// </summary>
    IObservable<DataSourceLink> DeletedDirectory { get; }

    /// <summary>
    /// Emits the event args of a changed file or directory in the data source.
    /// </summary>
    IObservable<DataSourceLink> Changed { get; }
    /// <summary>
    /// Emits the event args of a changed file in the data source.
    /// </summary>
    IObservable<DataSourceLink> ChangedFile { get; }
    /// <summary>
    /// Emits the event args of a changed directory in the data source.
    /// </summary>
    IObservable<DataSourceLink> ChangedDirectory { get; }

    /// <summary>
    /// Emits the event args of a renamed file or directory in the data source.
    /// </summary>
    IObservable<(DataSourceLink Old, DataSourceLink New)> Renamed { get; }
    /// <summary>
    /// Emits the event args of a renamed file in the data source.
    /// </summary>
    IObservable<(DataSourceLink Old, DataSourceLink New)> RenamedFile { get; }
    /// <summary>
    /// Emits the event args of a renamed directory in the data source.
    /// </summary>
    IObservable<(DataSourceLink Old, DataSourceLink New)> RenamedDirectory { get; }
}
