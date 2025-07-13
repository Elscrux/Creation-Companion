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
    IObservable<IDataSourceLink> Created { get; }
    /// <summary>
    /// Emits the event args of a newly created file in the data source.
    /// </summary>
    IObservable<DataSourceFileLink> CreatedFile { get; }
    /// <summary>
    /// Emits the event args of a newly created directory in the data source.
    /// </summary>
    IObservable<DataSourceDirectoryLink> CreatedDirectory { get; }

    /// <summary>
    /// Emits the event args of a deleted file or directory in the data source.
    /// </summary>
    IObservable<IDataSourceLink> Deleted { get; }
    /// <summary>
    /// Emits the event args of a deleted file in the data source.
    /// </summary>
    IObservable<DataSourceFileLink> DeletedFile { get; }
    /// <summary>
    /// Emits the event args of a deleted directory in the data source.
    /// </summary>
    IObservable<DataSourceDirectoryLink> DeletedDirectory { get; }

    /// <summary>
    /// Emits the event args of a changed file or directory in the data source.
    /// </summary>
    IObservable<IDataSourceLink> Changed { get; }
    /// <summary>
    /// Emits the event args of a changed file in the data source.
    /// </summary>
    IObservable<DataSourceFileLink> ChangedFile { get; }
    /// <summary>
    /// Emits the event args of a changed directory in the data source.
    /// </summary>
    IObservable<DataSourceDirectoryLink> ChangedDirectory { get; }

    /// <summary>
    /// Emits the event args of a renamed file or directory in the data source.
    /// </summary>
    IObservable<IUpdate<IDataSourceLink>> Renamed { get; }
    /// <summary>
    /// Emits the event args of a renamed file in the data source.
    /// </summary>
    IObservable<IUpdate<DataSourceFileLink>> RenamedFile { get; }
    /// <summary>
    /// Emits the event args of a renamed directory in the data source.
    /// </summary>
    IObservable<IUpdate<DataSourceDirectoryLink>> RenamedDirectory { get; }
}
