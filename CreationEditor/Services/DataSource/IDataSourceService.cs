using System.Diagnostics.CodeAnalysis;
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
    /// Data sources are sorted from least
    /// </summary>
    IReadOnlyList<IDataSource> PriorityOrder { get; }

    /// <summary>
    /// Active data source to be used as main data source to save files in.
    /// </summary>
    IDataSource ActiveDataSource { get; }

    /// <summary>
    /// Returns the data source for the given path.
    /// </summary>
    /// <param name="dataSourcePath">Path to the data source.</param>
    /// <param name="dataSource">Data source to return.</param>
    /// <returns>True if the data source was found, false otherwise.</returns>
    bool TryGetDataSource(string dataSourcePath, [NotNullWhen(true)] out IDataSource? dataSource);

    /// <summary>
    /// Adds a data source at the of the load order.
    /// </summary>
    /// <param name="dataSource">Data source to add.</param>
    void AddDataSource(IDataSource dataSource);

    /// <summary>
    /// Moves a data source to the given index.
    /// </summary>
    /// <param name="dataSource">Data source to move.</param>
    /// <param name="newIndex">New index to move the data source to.</param>
    void MoveDataSource(IDataSource dataSource, int newIndex);

    /// <summary>
    /// Tries to get the file system link for the given data relative path.
    /// The first data source in the load order that contains the file will be used.
    /// </summary>
    /// <param name="dataRelativePath">Data relative path to the file.</param>
    /// <returns>File system link to the file if found, null otherwise.</returns>
    FileSystemLink? GetFileLink(DataRelativePath dataRelativePath);

    /// <summary>
    /// Tries to get the file system link for the given data relative path.
    /// The first data source in the load order that contains the file will be used.
    /// </summary>
    /// <param name="dataRelativePath">Data relative path to the file.</param>
    /// <param name="link">File system link to the file if found, null otherwise.</param>
    /// <returns>>True if the file system link was found in any data source, false otherwise.</returns>
    bool TryGetFileLink(DataRelativePath dataRelativePath, [NotNullWhen(true)] out FileSystemLink? link);

    /// <summary>
    /// Enumerates all file system links in the given directory.
    /// In case of duplicate files, the file from the highest priority data source will be returned.
    /// </summary>
    /// <param name="directoryPath">Data relative path to the directory.</param>
    /// <param name="includeSubDirectories">Whether to include subdirectories.</param>
    /// <returns>File system links in the given directory.</returns>
    IEnumerable<FileSystemLink> EnumerateFileLinksInAllDataSources(DataRelativePath directoryPath, bool includeSubDirectories);

    /// <summary>
    /// Checks if the given data relative path exists in any data source.
    /// </summary>
    /// <param name="path">Data relative path to check.</param>
    /// <returns>>True if the data relative path exists in any data source, false otherwise.</returns>
    bool FileExists(DataRelativePath path);
}
