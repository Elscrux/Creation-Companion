using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Reactive.Subjects;
using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Environment;
using CreationEditor.Services.State;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.DataSource;

// TODO: It would be good to automatically distribute changes in a plugin per master plugin. What I mean is, if you're rerouting a bunch of mesh paths in different esms, having a seperate esp for the records in each plugin rather than one big esp would be very useful in our case
// but still possible probably when you have a point that pipes changes to certain records to certain plugins - so a change to BSHeartland.esm goes to EditedBSH.esp and so on
// so you'd check a box that pipes every edit to an immutable mod into a dedicated mod that edits one plugin each there
// maybe just for custom code for BS and no actual system behind it for everyone to use
public sealed class DataSourceService : IDataSourceService {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IArchiveService _archiveService;
    private readonly List<IDataSource> _dataSources = [];
    public IReadOnlyList<IDataSource> PriorityOrder => _dataSources.ToArray();

    /// <summary>
    /// Orders archives before file system data sources and keeps the order within the same type of data source.
    /// </summary>
    private readonly FuncComparer<IDataSource> _dataSourceComparer;

    /// <summary>
    /// Orders archives based on the load order of the bsa files in the data directory.
    /// </summary>
    private readonly IComparer<FileSystemLink> _archivePriorityComparer;

    private readonly ReplaySubject<IReadOnlyList<IDataSource>> _dataSourcesChanged = new(1);
    private readonly IStateRepository<DataSourceMemento> _stateRepository;
    public IObservable<IReadOnlyList<IDataSource>> DataSourcesChanged => _dataSourcesChanged;

    public DataSourceService(
        IFileSystem fileSystem,
        IEditorEnvironment editorEnvironment,
        IDataDirectoryProvider dataDirectoryProvider,
        IArchiveService archiveService,
        Func<string, IStateRepository<DataSourceMemento>> stateRepositoryFactory
    ) {
        _editorEnvironment = editorEnvironment;
        _archiveService = archiveService;
        _stateRepository = stateRepositoryFactory("DataSource");

        _archivePriorityComparer = new FuncComparer<FileSystemLink>((x, y) => {
            // Collect bsa files in the data directory and sort them based on the load order
            var bsaNameOrder = _editorEnvironment.GameEnvironment.LinkCache.ListedOrder
                .SelectMany(GetModBSAFiles)
                .ToArray();

            // todo potentially filter out bsa files that are not in the load order or referenced in the ini file
            var indexOfX = bsaNameOrder.IndexOf(x.DataRelativePath.Path);
            var indexOfY = bsaNameOrder.IndexOf(y.DataRelativePath.Path);

            if (indexOfX == -1) {
                if (indexOfY == -1) return 0;

                return -1;
            }

            if (indexOfY == -1) return 1;

            // Files that are higher in the load order will have a higher index and be prioritized 
            return indexOfX.CompareTo(indexOfY);

            IEnumerable<string> GetModBSAFiles(IModGetter mod) {
                var extension = _archiveService.GetExtension();

                yield return mod.ModKey.Name + extension;
                yield return mod.ModKey.Name + " - Textures" + extension;
            }
        });

        _dataSourceComparer = new FuncComparer<IDataSource>((s1, s2) => {
            if (s1 is ArchiveDataSource) {
                if (s2 is ArchiveDataSource) return 0;

                return -1;
            }

            if (s2 is ArchiveDataSource) return 1;

            return 0;
        });

        InitDataSources(fileSystem, dataDirectoryProvider, archiveService);
    }

    private void InitDataSources(IFileSystem fileSystem, IDataDirectoryProvider dataDirectoryProvider, IArchiveService archiveService) {
        List<FileSystemDataSource> fileSystemDataSources = [];
        List<ArchiveDataSource> archiveDataSources = [];

        var mementos = _stateRepository.LoadAll().ToArray();

        foreach (var dataSourceMemento in mementos.Where(x => x.Type == DataSourceType.FileSystem)) {
            fileSystemDataSources.Add(new FileSystemDataSource(fileSystem, dataSourceMemento.Path, dataSourceMemento.IsReadyOnly));
        }

        // If there is no data folder source, add it
        if (!fileSystemDataSources.Any(fs => DataRelativePath.PathComparer.Equals(fs.Path, dataDirectoryProvider.Path))) {
            var dataFolderDataSource = new FileSystemDataSource(fileSystem, dataDirectoryProvider.Path);
            fileSystemDataSources.Insert(0, dataFolderDataSource);
        }

        foreach (var dataSourceMemento in mementos.Where(x => x.Type == DataSourceType.Archive)) {
            var archiveDataSourcePath = fileSystem.Path.GetDirectoryName(dataSourceMemento.Path);
            if (archiveDataSourcePath is null) continue;

            var dataSource = fileSystemDataSources.FirstOrDefault(dataSource => dataSource.Path == archiveDataSourcePath);
            if (dataSource is null) continue;

            var archiveLink = new FileSystemLink(dataSource, fileSystem.Path.GetFileName(dataSourceMemento.Path));
            var archiveDataSource = new ArchiveDataSource(fileSystem, dataSourceMemento.Path, archiveService.GetReader(archiveLink), archiveLink);
            archiveDataSources.Add(archiveDataSource);
        }

        // If any loaded archive is not added, add it
        foreach (var archive in GetArchives(fileSystemDataSources)) {
            var archiveDataSource = new ArchiveDataSource(fileSystem, archive.FullPath, archiveService.GetReader(archive), archive);
            archiveDataSources.Add(archiveDataSource);
        }

        archiveDataSources = archiveDataSources
            .OrderBy(dataSource => dataSource.ArchiveLink, _archivePriorityComparer)
            .ToList();

        var dataSources = archiveDataSources
            .Concat<IDataSource>(fileSystemDataSources)
            .ToArray();

        _dataSources.AddRange(dataSources);
    }

    public bool TryGetDataSource(string dataSourcePath, [NotNullWhen(true)] out IDataSource? dataSource) {
        dataSource = _dataSources.FirstOrDefault(x => DataRelativePath.PathComparer.Equals(x.Path, dataSourcePath));
        return dataSource is not null;
    }

    public void AddDataSource(IDataSource dataSource) {
        _dataSources.Add(dataSource);
        OnDataSourcesChanged();
    }

    public void MoveDataSource(IDataSource dataSource, int newIndex) {
        // todo check if the movement is still keeping a valid order
        if (!_dataSources.Contains(dataSource)) return;

        var index = _dataSources.IndexOf(dataSource);
        if (index == newIndex) return;

        _dataSources.RemoveAt(index);
        _dataSources.Insert(newIndex < index ? newIndex : newIndex - 1, dataSource);
        OnDataSourcesChanged();
    }

    public bool TryGetFileLink(DataRelativePath dataRelativePath, [NotNullWhen(true)] out FileSystemLink? link) {
        var firstMatch = _dataSources.FirstOrDefault(d => d.FileExists(dataRelativePath));
        if (firstMatch is null) {
            link = null;
            return false;
        }

        link = new FileSystemLink(firstMatch, dataRelativePath);
        return true;
    }

    public IEnumerable<FileSystemLink> EnumerateFileLinksInAllDataSources(DataRelativePath directoryPath, bool includeSubDirectories) {
        var referencedFiles = new HashSet<DataRelativePath>();

        foreach (var dataSource in _dataSources) {
            var fullPath = dataSource.GetFullPath(directoryPath);
            if (!dataSource.FileSystem.Directory.Exists(fullPath)) continue;

            var files = dataSource.FileSystem.Directory.EnumerateFiles(
                fullPath,
                "*",
                includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (var file in files) {
                if (referencedFiles.Contains(file)) continue;

                var dataRelativePath = new DataRelativePath(file);
                referencedFiles.Add(dataRelativePath);
                yield return new FileSystemLink(dataSource, dataRelativePath);
            }
        }
    }

    public bool FileExists(DataRelativePath path) {
        return _dataSources.Any(dataSource => dataSource.FileExists(path));
    }

    private void OnDataSourcesChanged() {
        foreach (var dataSource in PriorityOrder) {
            var memento = dataSource.CreateMemento();
            _stateRepository.Save(memento, Guid.NewGuid(), memento.Name);
        }

        _dataSourcesChanged.OnNext(PriorityOrder);
    }

    private IEnumerable<FileSystemLink> GetArchives(IReadOnlyList<IDataSource> dataSources) {
        var compareArchiveNames = new FuncComparer<FileSystemLink>((x, y) => {
            // Collect bsa files in the data directory and sort them based on the load order
            var bsaNameOrder = _editorEnvironment.GameEnvironment.LinkCache.ListedOrder
                .SelectMany(GetModBSAFiles)
                .ToArray();

            // todo potentially filter out bsa files that are not in the load order or referenced in the ini file
            var indexOfX = bsaNameOrder.IndexOf(x.DataRelativePath.Path);
            var indexOfY = bsaNameOrder.IndexOf(y.DataRelativePath.Path);

            if (indexOfX == -1) {
                if (indexOfY == -1) return 0;

                return -1;
            }

            if (indexOfY == -1) return 1;

            // Files that are higher in the load order will have a higher index and be prioritized 
            return indexOfX.CompareTo(indexOfY);

            IEnumerable<string> GetModBSAFiles(IModGetter mod) {
                var extension = _archiveService.GetExtension();

                yield return mod.ModKey.Name + extension;
                yield return mod.ModKey.Name + " - Textures" + extension;
            }
        });

        return dataSources
            .Select(dataSource => new FileSystemLink(dataSource, string.Empty))
            .SelectMany(rootLink => rootLink.EnumerateFileLinks("*" + _archiveService.GetExtension(), false))
            .Order(compareArchiveNames);
    }
}
