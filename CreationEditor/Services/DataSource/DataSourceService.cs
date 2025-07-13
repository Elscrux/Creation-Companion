using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Reactive.Subjects;
using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Archive;
using CreationEditor.Services.State;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Services.DataSource;

public sealed class DataSourceService : IDataSourceService {
    private readonly IFileSystem _fileSystem;
    private readonly IArchiveService _archiveService;
    private readonly List<IDataSource> _dataSources = [];
    public IReadOnlyList<IDataSource> ListedOrder => _dataSources.ToArray();
    public IReadOnlyList<IDataSource> PriorityOrder => ListedOrder.Reverse().ToArray();
    public IDataSource ActiveDataSource { get; private set; }

    public FuncComparer<IDataSource> DataSourceComparer { get; }

    /// <summary>
    /// Orders archives based on the load order of the archive files in the data directory.
    /// </summary>
    private readonly IComparer<DataSourceFileLink> _archivePriorityComparer;

    private readonly ReplaySubject<IReadOnlyList<IDataSource>> _dataSourcesChanged = new(1);
    private readonly IStateRepository<DataSourceMemento, DataSourceMemento, NamedGuid> _stateRepository;
    public IObservable<IReadOnlyList<IDataSource>> DataSourcesChanged => _dataSourcesChanged;

    public DataSourceService(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        IArchiveService archiveService,
        IStateRepositoryFactory<DataSourceMemento, DataSourceMemento, NamedGuid> stateRepositoryFactory
    ) {
        _fileSystem = fileSystem;
        _archiveService = archiveService;
        _stateRepository = stateRepositoryFactory.Create("DataSource");

        _archivePriorityComparer = new FuncComparer<DataSourceFileLink>((x, y) => {
            // Collect archive files in the data directory and sort them based on the load order
            var archiveLoadOrder = archiveService.GetArchiveLoadOrder().ToArray();

            // todo potentially filter out archive files that are not in the load order or referenced in the ini file
            var indexOfX = archiveLoadOrder.IndexOf(x.DataRelativePath.Path);
            var indexOfY = archiveLoadOrder.IndexOf(y.DataRelativePath.Path);

            if (indexOfX == -1) {
                if (indexOfY == -1) return 0;

                return -1;
            }

            if (indexOfY == -1) return 1;

            // Files that are higher in the load order will have a higher index and be prioritized 
            return indexOfX.CompareTo(indexOfY);
        });

        DataSourceComparer = new FuncComparer<IDataSource>((s1, s2) => {
            return s1 switch {
                ArchiveDataSource a1 => s2 switch {
                    ArchiveDataSource a2 => _archivePriorityComparer.Compare(a1.ArchiveFileLink, a2.ArchiveFileLink),
                    _ => -1,
                },
                FileSystemDataSource => s2 switch {
                    ArchiveDataSource => 1,
                    _ => IndexOfOrMax(s1).CompareTo(IndexOfOrMax(s2)),
                },
                _ => IndexOfOrMax(s1).CompareTo(IndexOfOrMax(s2)),
            };

            int IndexOfOrMax(IDataSource dataSource) {
                var indexOf = _dataSources.IndexOf(dataSource);
                return indexOf == -1 ? int.MaxValue : indexOf;
            }
        });

        var (archiveDataSources, fileSystemDataSources) = GetMementoDataSources();

        // If there is no data folder source, add it
        var dataDirectoryDataSource =
            fileSystemDataSources.FirstOrDefault(fs => DataRelativePath.PathComparer.Equals(fs.Path, dataDirectoryProvider.Path));
        if (dataDirectoryDataSource is null) {
            dataDirectoryDataSource = new FileSystemDataSource(fileSystem, dataDirectoryProvider.Path);
        } else {
            // Move the data directory data source to the front of the list
            fileSystemDataSources.Remove(dataDirectoryDataSource);
        }

        fileSystemDataSources.Insert(0, dataDirectoryDataSource);

        ActiveDataSource = dataDirectoryDataSource;

        UpdateDataSources(archiveDataSources.Concat<IDataSource>(fileSystemDataSources).ToArray(), ActiveDataSource);
    }

    private (List<ArchiveDataSource> ArchiveDataSources, List<FileSystemDataSource> FileSystemDatSources) GetMementoDataSources() {
        List<FileSystemDataSource> fileSystemDataSources = [];
        List<ArchiveDataSource> archiveDataSources = [];

        var mementos = _stateRepository.LoadAll()
            .DistinctBy(x => x.Path)
            .ToArray();

        // Add file system data sources from mementos
        foreach (var (_, _, path, isReadyOnly) in mementos.Where(x => x.Type == DataSourceType.FileSystem)) {
            if (_fileSystem.Directory.Exists(path)) {
                fileSystemDataSources.Add(new FileSystemDataSource(_fileSystem, path, isReadyOnly));
            }
        }

        // Add archive data sources from mementos
        foreach (var (_, _, path, _) in mementos.Where(x => x.Type == DataSourceType.Archive)) {
            var archiveDataSourcePath = _fileSystem.Path.GetDirectoryName(path);
            if (archiveDataSourcePath is null) continue;

            var dataSource = fileSystemDataSources.FirstOrDefault(dataSource => dataSource.Path == archiveDataSourcePath);
            if (dataSource is null) continue;

            var archiveLink = new DataSourceFileLink(dataSource, _fileSystem.Path.GetFileName(path));
            if (archiveLink.Exists()) {
                var archiveDataSource = new ArchiveDataSource(_fileSystem, path, _archiveService.GetReader(archiveLink), archiveLink);
                archiveDataSources.Add(archiveDataSource);
            }
        }

        return (archiveDataSources, fileSystemDataSources);
    }

    public bool TryGetDataSource(string dataSourcePath, [NotNullWhen(true)] out IDataSource? dataSource) {
        dataSource = PriorityOrder.FirstOrDefault(x => DataRelativePath.PathComparer.Equals(x.Path, dataSourcePath));
        return dataSource is not null;
    }

    public DataSourceFileLink? GetFileLink(string path) {
        if (!_fileSystem.Path.IsPathRooted(path)) return GetFileLink(new DataRelativePath(path));

        var dataSource = PriorityOrder.FirstOrDefault(d => path.StartsWith(d.Path, DataRelativePath.PathComparison));
        if (dataSource is null) return null;

        var relativePath = dataSource.FileSystem.Path.GetRelativePath(dataSource.Path, path);
        return new DataSourceFileLink(dataSource, relativePath);
    }

    public DataSourceFileLink? GetFileLink(DataRelativePath dataRelativePath) {
        var firstMatch = PriorityOrder.FirstOrDefault(d => d.FileExists(dataRelativePath));
        if (firstMatch is null) return null;

        return new DataSourceFileLink(firstMatch, dataRelativePath);
    }

    public bool TryGetFileLink(DataRelativePath dataRelativePath, [NotNullWhen(true)] out DataSourceFileLink? link) {
        link = GetFileLink(dataRelativePath);
        return link is not null;
    }

    public IEnumerable<DataSourceFileLink> EnumerateFileLinksInAllDataSources(
        DataRelativePath directoryPath,
        bool includeSubDirectories,
        string searchPattern = "*") {
        var referencedFiles = new HashSet<DataRelativePath>();

        foreach (var dataSource in PriorityOrder) {
            if (!dataSource.DirectoryExists(directoryPath)) continue;

            var files = dataSource.EnumerateFiles(directoryPath, searchPattern, includeSubDirectories);

            foreach (var dataRelativePath in files) {
                if (!referencedFiles.Add(dataRelativePath)) continue;

                yield return new DataSourceFileLink(dataSource, dataRelativePath);
            }
        }
    }

    public bool FileExists(DataRelativePath path) {
        return _dataSources.Any(dataSource => dataSource.FileExists(path));
    }

    public void UpdateDataSources(IReadOnlyList<IDataSource> dataSources, IDataSource activeDataSource) {
        if (!dataSources.Contains(activeDataSource))
            throw new ArgumentException("Active data source must be part of the provided data sources.", nameof(activeDataSource));

        var fileSystemDataSources = dataSources.OfType<FileSystemDataSource>().ToList();
        var archiveDataSources = dataSources.OfType<ArchiveDataSource>().ToList();

        // Load archive data sources found in file system data sources
        archiveDataSources = archiveDataSources
            .Concat(GetNestedArchiveDataSources(fileSystemDataSources))
            .DistinctBy(x => x.Path)
            .OrderBy(dataSource => dataSource.ArchiveFileLink, _archivePriorityComparer)
            .ToList();

        // Create order where archives are loaded first, and then overwritten by file system data sources
        dataSources = archiveDataSources
            .Concat<IDataSource>(fileSystemDataSources)
            .ToArray();

        _dataSources.Clear();
        _dataSources.AddRange(dataSources);

        ActiveDataSource = activeDataSource;

        OnDataSourcesChanged();
    }

    public IEnumerable<ArchiveDataSource> GetNestedArchiveDataSources(IEnumerable<IDataSource> dataSources) {
        return GetArchives(dataSources.OfType<FileSystemDataSource>())
            .Select(archive => new ArchiveDataSource(_fileSystem, archive.FullPath, _archiveService.GetReader(archive), archive));
    }

    private void OnDataSourcesChanged() {
        var existingStateIdentifiers = _stateRepository.LoadAllWithIdentifier().ToDictionary(x => x.Key, x => x.Value);

        // Add or update mementos for each data source in the priority order
        foreach (var dataSource in PriorityOrder) {
            var existing = existingStateIdentifiers.FirstOrDefault(x => x.Value.Path == dataSource.Path);
            var id = existing.Value is null ? Guid.NewGuid() : existing.Key.Id;

            var memento = dataSource.CreateMemento();
            _stateRepository.Save(memento, new NamedGuid(id, dataSource.Name));
        }

        // Remove any mementos that are not in the current data sources
        foreach (var (id, memento) in existingStateIdentifiers) {
            if (_dataSources.Any(ds => ds.Path == memento.Path)) continue;

            _stateRepository.Delete(id);
        }

        _dataSourcesChanged.OnNext(ListedOrder);
    }

    private IEnumerable<DataSourceFileLink> GetArchives(IEnumerable<FileSystemDataSource> dataSources) {
        return dataSources
            .SelectMany(dataSource => dataSource.GetRootLink()
                .EnumerateFileLinks("*" + _archiveService.GetExtension(), false));
    }
}
