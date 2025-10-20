using System.Diagnostics.CodeAnalysis;
using CreationEditor.Resources.Comparer;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

/// <summary>
/// Acts as the main way to access data inside something that represents a data folder.
/// <example>
/// <list type="bullet">
///  <item>the game's data folder</item>
///  <item>a mod folder that is virtually injected into the game's data folder</item>
///  <item>an archive</item>
/// </list>
/// </example>
/// </summary>
public interface IDataSourceService {
    /// <summary>
    /// Observable that is triggered when there is any change to the data sources.
    /// </summary>
    IObservable<IReadOnlyList<IDataSource>> DataSourcesChanged { get; }

    /// <summary>
    /// Returns a list of all data sources.
    /// Data sources are sorted from the highest priority to the lowest priority.
    /// </summary>
    IReadOnlyList<IDataSource> PriorityOrder { get; }

    /// <summary>
    /// Returns a list of all data sources.
    /// Data sources are sorted from the lowest priority to the highest priority.
    /// </summary>
    IReadOnlyList<IDataSource> ListedOrder { get; }

    /// <summary>
    /// Active data source to be used as main data source to save files in.
    /// </summary>
    IDataSource ActiveDataSource { get; }

    /// <summary>
    /// Data source that represents the actual data directory of the game.
    /// </summary>
    FileSystemDataSource DataDirectoryDataSource { get; }

    /// <summary>
    /// Ensures load order of data sources based on certain rules.
    /// </summary>
    FuncComparer<IDataSource> DataSourceComparer { get; }

    /// <summary>
    /// Returns the data source for the given path.
    /// </summary>
    /// <param name="dataSourcePath">Path to the data source.</param>
    /// <param name="dataSource">Data source to return.</param>
    /// <returns>True if the data source was found, false otherwise.</returns>
    bool TryGetDataSource(string dataSourcePath, [NotNullWhen(true)] out IDataSource? dataSource);

    /// <summary>
    /// Tries to get the file system link for the given path.
    /// In case of an absolute file path, it will be separated into the data source and the relative path.
    /// If the data source exists, the file system link will be returned.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <returns>File system link to the file if found, null otherwise.</returns>
    DataSourceFileLink? GetFileLink(string path);

    /// <summary>
    /// Tries to get the file system link for the given data relative path.
    /// The first data source in the load order that contains the file will be used.
    /// </summary>
    /// <param name="dataRelativePath">Data relative path to the file.</param>
    /// <returns>File system link to the file if found, null otherwise.</returns>
    DataSourceFileLink? GetFileLink(DataRelativePath dataRelativePath);

    /// <summary>
    /// Tries to get the file system link for the given data relative path.
    /// The first data source in the load order that contains the file will be used.
    /// </summary>
    /// <param name="dataRelativePath">Data relative path to the file.</param>
    /// <param name="link">File system link to the file if found, null otherwise.</param>
    /// <returns>>True if the file system link was found in any data source, false otherwise.</returns>
    bool TryGetFileLink(DataRelativePath dataRelativePath, [NotNullWhen(true)] out DataSourceFileLink? link);

    /// <summary>
    /// Enumerates all file system links in the given directory.
    /// In case of duplicate files, the file from the highest priority data source will be returned.
    /// </summary>
    /// <param name="directoryPath">Data relative path to the directory.</param>
    /// <param name="includeSubDirectories">Whether to include subdirectories.</param>
    /// <param name="searchPattern">Search pattern to filter files.</param>
    /// <returns>File system links in the given directory.</returns>
    IEnumerable<DataSourceFileLink> EnumerateFileLinksInAllDataSources(DataRelativePath directoryPath, bool includeSubDirectories, string searchPattern = "*");

    /// <summary>
    /// Checks if the given data relative path exists in any data source.
    /// </summary>
    /// <param name="path">Data relative path to check.</param>
    /// <returns>>True if the data relative path exists in any data source, false otherwise.</returns>
    bool FileExists(DataRelativePath path);

    /// <summary>
    /// Replaces the current data sources with the given list.
    /// </summary>
    /// <param name="dataSources">List of data sources to set.</param>
    /// <param name="activeDataSource">Optional active data source to set.</param>
    void UpdateDataSources(IReadOnlyList<IDataSource> dataSources, IDataSource activeDataSource);

    /// <summary>
    /// Gets the archive data sources from within the given data sources.
    /// </summary>
    /// <param name="dataSources">Data sources to search for archive data sources.</param>
    /// <returns>Enumerable of archive data sources found in the given data sources.</returns>
    IEnumerable<ArchiveDataSource> GetNestedArchiveDataSources(IEnumerable<IDataSource> dataSources);
}
