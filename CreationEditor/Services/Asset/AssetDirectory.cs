using System.IO.Abstractions;
using System.Reactive.Linq;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Mutagen.References.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using DynamicData;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class AssetDirectory : IAsset {
    private readonly DisposableBucket _disposables = new();

    private readonly ILogger _logger;
    private readonly IAssetTypeService _assetTypeService;
    private readonly IArchiveService _archiveService;
    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryService _dataDirectoryService;
    private readonly IAssetReferenceController _assetReferenceController;

    public IDirectoryInfo Directory { get; }
    public string Path => Directory.FullName;

    public bool IsDirectory => true;
    public bool HasChildren {
        get {
            if (!_loadedAssets) LoadAssets();

            return Assets.Count > 0;
        }
    }
    public bool IsVirtual { get; }

    public IEnumerable<IAsset> Children {
        get {
            if (!_loadedAssets) LoadAssets();

            return Assets.Items;
        }
    }

    private bool _loadedAssets;
    public SourceCache<IAsset, string> Assets { get; } = new(a => a.Path);

    public AssetDirectory(
        IDirectoryInfo directory,
        ILogger logger,
        IFileSystem fileSystem,
        IDataDirectoryService dataDirectoryService,
        IAssetReferenceController assetReferenceController,
        IAssetTypeService assetTypeService,
        IArchiveService archiveService,
        bool isVirtual) {
        _logger = logger;
        _assetTypeService = assetTypeService;
        _archiveService = archiveService;
        _fileSystem = fileSystem;
        _dataDirectoryService = dataDirectoryService;
        _assetReferenceController = assetReferenceController;
        Directory = directory;
        IsVirtual = isVirtual;
    }

    public AssetDirectory(
        IDirectoryInfo directory,
        ILogger logger,
        IFileSystem fileSystem,
        IDataDirectoryService dataDirectoryService,
        IAssetReferenceController assetReferenceController,
        IAssetTypeService assetTypeService,
        IArchiveService archiveService)
        : this(
            directory,
            logger,
            fileSystem,
            dataDirectoryService,
            assetReferenceController,
            assetTypeService,
            archiveService,
            !fileSystem.Path.Exists(directory.FullName)) {}

    private void Add(string path) {
        try {
            var directoryInfo = _fileSystem.DirectoryInfo.New(path);
            if ((directoryInfo.Attributes & FileAttributes.Directory) == 0) {
                var asset = FileToAsset(path);
                if (asset is null) return;

                asset.DisposeWith(_disposables);

                _assetReferenceController.RegisterCreation(asset);
                Assets.AddOrUpdate(asset);
            } else {
                var assetDirectory = new AssetDirectory(
                    directoryInfo,
                    _logger,
                    _fileSystem,
                    _dataDirectoryService,
                    _assetReferenceController,
                    _assetTypeService,
                    _archiveService,
                    false);
                assetDirectory.DisposeWith(_disposables);
                Assets.AddOrUpdate(assetDirectory);
            }
        } catch (Exception) {
            Refresh();
        }
    }

    private void Remove(string path) {
        try {
            var fileName = _fileSystem.Path.GetFileName(path);
            using var asset = Assets.Items.FirstOrDefault(asset => {
                var assetName = _fileSystem.Path.GetFileName(asset.Path);
                return string.Equals(assetName, fileName, DataRelativePath.PathComparison);
            });
            if (asset is null) return;

            Assets.Remove(asset);
            if (asset is AssetFile file) {
                _assetReferenceController.RegisterDeletion(file);
            } else {
                var childFiles = asset.Children
                    .GetAllChildren(x => x.Children)
                    .OfType<AssetFile>();

                foreach (var childFile in childFiles) {
                    _assetReferenceController.RegisterDeletion(childFile);
                }
            }
        } catch (Exception) {
            Refresh();
        }
    }

    private void Change(string path) {
        try {
            var fileName = _fileSystem.Path.GetFileName(path);
            var asset = Assets.Items.FirstOrDefault(asset => {
                var assetName = _fileSystem.Path.GetFileName(asset.Path);
                return string.Equals(assetName, fileName, DataRelativePath.PathComparison);
            });
            if (asset is not AssetFile file) return;

            var registerUpdate = _assetReferenceController.RegisterUpdate(file);
            registerUpdate(file);
        } catch (Exception) {
            Refresh();
        }
    }

    private bool IsFileRelevant(IFileInfo file) {
        return _assetTypeService.FileExtensions.Contains(file.Extension);
    }

    private bool IsFileRelevant(string file) {
        var extension = _fileSystem.Path.GetExtension(file);

        return _assetTypeService.FileExtensions.Contains(extension);
    }

    public void Dispose() => _disposables.Dispose();

    public void LoadAssets() {
        if (_loadedAssets) return;

        _loadedAssets = true;

        Refresh();

        if (!IsVirtual) {
            // potential performance issue - every loaded directory will check if a file change is relevant
            // instead, it might be best to subscribe just for changes in the current directory
            // would require a change in the data directory service api
            _dataDirectoryService.Created
                .Where(e => Path.Equals(_fileSystem.Path.GetDirectoryName(e.FullPath), DataRelativePath.PathComparison))
                .ObserveOnGui()
                .Subscribe(e => Add(e.FullPath))
                .DisposeWith(_disposables);

            _dataDirectoryService.Deleted
                .Where(e => Path.Equals(_fileSystem.Path.GetDirectoryName(e.FullPath), DataRelativePath.PathComparison))
                .ObserveOnGui()
                .Subscribe(e => Remove(e.FullPath))
                .DisposeWith(_disposables);

            _dataDirectoryService.Renamed
                .Where(e => Path.Equals(_fileSystem.Path.GetDirectoryName(e.FullPath), DataRelativePath.PathComparison))
                .ObserveOnGui()
                .Subscribe(e => {
                    Remove(e.OldFullPath);
                    Add(e.FullPath);
                })
                .DisposeWith(_disposables);

            _dataDirectoryService.Changed
                .Where(e => Path.Equals(_fileSystem.Path.GetDirectoryName(e.FullPath), DataRelativePath.PathComparison))
                .ObserveOnGui()
                .Subscribe(e => Change(e.FullPath))
                .DisposeWith(_disposables);
        }
    }

    private void Refresh() {
        var addedDirectories = new HashSet<string>(DataRelativePath.PathComparer);
        IEnumerable<IAsset> assets = [];
        if (!IsVirtual) {
            assets = assets.Concat(Directory.EnumerateDirectories()
                .Select(dirPath => addedDirectories.Add(dirPath.Name)
                    ? new AssetDirectory(
                        dirPath,
                        _logger,
                        _fileSystem,
                        _dataDirectoryService,
                        _assetReferenceController,
                        _assetTypeService,
                        _archiveService,
                        false)
                    : null)
                .WhereNotNull());
        }

        assets = assets.Concat(_archiveService.GetSubdirectories(Path)
            .Select(dirPath => _fileSystem.DirectoryInfo.New(dirPath))
            .Select(dirInfo => addedDirectories.Add(dirInfo.Name)
                ? new AssetDirectory(
                    dirInfo,
                    _logger,
                    _fileSystem,
                    _dataDirectoryService,
                    _assetReferenceController,
                    _assetTypeService,
                    _archiveService,
                    true)
                : null)
            .WhereNotNull());

        var addedFiles = new HashSet<string>();
        if (!IsVirtual) {
            assets = assets.Concat(Directory.EnumerateFiles()
                .Where(IsFileRelevant)
                .Select(file => {
                    var asset = FileToAsset(file.FullName);
                    return addedFiles.Add(file.Name) ? asset : null;
                })
                .WhereNotNull());
        }

        assets = _archiveService.GetFilesInDirectory(Directory.FullName)
            .Where(IsFileRelevant)
            .Select(file => {
                var asset = FileToAsset(file, true);
                return addedFiles.Add(_fileSystem.Path.GetFileName(file)) ? asset : null;
            })
            .WhereNotNull()
            .Concat(assets)
            .ToList();

        Assets.Edit(updater => {
            updater.Clear();
            foreach (var asset in assets) {
                updater.AddOrUpdate(asset);
            }
        });
    }

    private AssetFile? FileToAsset(string file, bool isVirtual = false) {
        IAssetLink? assetLink;
        try {
            assetLink = _assetTypeService.GetAssetLink(file);
        } catch (AssetPathMisalignedException e) {
            _logger.Here().Warning(e, "Failed to parse asset path {Path}: {Exception}", file, e.Message);
            return null;
        }
        if (assetLink is null) return null;

        _assetReferenceController.GetReferencedAsset(assetLink, out var referencedAsset).DisposeWith(_disposables);
        var fileName = _fileSystem.Path.Combine(Path, _fileSystem.Path.GetFileName(assetLink.GivenPath));
        return new AssetFile(fileName, referencedAsset, isVirtual);
    }

    public IEnumerable<IReferencedAsset> GetReferencedAssets() {
        return this
            .GetAllChildren<IAsset>(a => a.Children)
            .OfType<AssetFile>()
            .Select(file => file.ReferencedAsset);
    }

    /// <summary>
    /// Checks if the given asset exists in this directory or any of its subdirectories.
    /// </summary>
    /// <param name="fullPath">Full path of the asset to check for</param>
    /// <returns>true if the asset exists under this directory</returns>
    public bool Contains(string fullPath) {
        return GetAssetFile(fullPath) is not null;
    }

    /// <summary>
    /// Returns the asset file with the given file path.
    /// </summary>
    /// <param name="filePath">file path of the asset file</param>
    /// <returns>asset file with the given file path</returns>
    public AssetFile? GetAssetFile(string filePath) {
        var relativePath = _fileSystem.Path.GetRelativePath(Path, filePath);
        if (relativePath.IsNullOrWhitespace()) return null;

        var currentDirectory = this;
        var directories = relativePath.Split(_fileSystem.Path.DirectorySeparatorChar, _fileSystem.Path.AltDirectorySeparatorChar).ToArray();

        foreach (var directory in directories.SkipLast(1)) {
            currentDirectory = currentDirectory.Children
                .OfType<AssetDirectory>()
                .FirstOrDefault(dir => string.Equals(_fileSystem.Path.GetFileName(dir.Path), directory, DataRelativePath.PathComparison));

            if (currentDirectory is null) break;
        }

        return currentDirectory?.Children
            .OfType<AssetFile>()
            .FirstOrDefault(file => string.Equals(_fileSystem.Path.GetFileName(file.Path), directories[^1], DataRelativePath.PathComparison));
    }

    /// <summary>
    /// Returns the asset file with the given file path.
    /// </summary>
    /// <param name="filePath">file path of the asset file</param>
    public AssetFile? this[string filePath] => GetAssetFile(filePath);
}
