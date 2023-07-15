﻿using System.IO.Abstractions;
using System.Reactive.Linq;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Mutagen.References.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using DynamicData;
using Noggog;
namespace CreationEditor.Services.Asset;

public sealed class AssetDirectory : IAsset {
    private readonly IAssetTypeService _assetTypeService;
    private readonly IArchiveService _archiveService;
    private readonly IFileSystem _fileSystem;
    private readonly IDeleteDirectoryProvider _deleteDirectoryProvider;
    private readonly IAssetReferenceController _assetReferenceController;
    private readonly IDisposableDropoff _disposables = new DisposableBucket();

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

    private IFileSystemWatcher? _watcher;

    public AssetDirectory(
        IDirectoryInfo directory,
        IFileSystem fileSystem,
        IDeleteDirectoryProvider deleteDirectoryProvider,
        IAssetReferenceController assetReferenceController,
        IAssetTypeService assetTypeService,
        IArchiveService archiveService,
        bool isVirtual = false) {
        _assetTypeService = assetTypeService;
        _archiveService = archiveService;
        _fileSystem = fileSystem;
        _deleteDirectoryProvider = deleteDirectoryProvider;
        _assetReferenceController = assetReferenceController;
        Directory = directory;
        IsVirtual = isVirtual;
    }

    private void Add(IFileSystem fileSystem, string path) {
        try {
            var directoryInfo = fileSystem.DirectoryInfo.New(path);
            if ((directoryInfo.Attributes & FileAttributes.Directory) == 0) {
                var asset = FileToAsset(path);
                if (asset is null) return;

                asset.DisposeWith(_disposables);

                _assetReferenceController.RegisterCreation(asset);
                Assets.AddOrUpdate(asset);
            } else {
                var assetDirectory = new AssetDirectory(directoryInfo, _fileSystem, _deleteDirectoryProvider, _assetReferenceController, _assetTypeService, _archiveService);
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
            var asset = Assets.Items.FirstOrDefault(asset => string.Equals(_fileSystem.Path.GetFileName(asset.Path), fileName, AssetCompare.PathComparison));
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
            asset.Dispose();
        } catch (Exception) {
            Refresh();
        }
    }

    private void Change(string path) {
        try {
            var fileName = _fileSystem.Path.GetFileName(path);
            var asset = Assets.Items.FirstOrDefault(asset => string.Equals(_fileSystem.Path.GetFileName(asset.Path), fileName, AssetCompare.PathComparison));
            if (asset is AssetFile file) {
                var registerUpdate = _assetReferenceController.RegisterUpdate(file);
                registerUpdate(file);
            }
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

    private void LoadAssets() {
        if (_loadedAssets) return;

        _loadedAssets = true;

        Refresh();

        if (!IsVirtual) {
            _watcher = _fileSystem.FileSystemWatcher.New(Directory.FullName).DisposeWith(_disposables);
            // _watcher.Filters.AddRange(_assetTypeService.GetFileMasks()); doesn't allow directory changes
            _watcher.IncludeSubdirectories = false;
            _watcher.EnableRaisingEvents = true;

            Observable
                .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                    h => _watcher.Created += h,
                    h => _watcher.Created -= h)
                .Where(e => !_fileSystem.Path.HasExtension(e.EventArgs.FullPath) || IsFileRelevant(e.EventArgs.FullPath))
                .ObserveOnGui()
                .Subscribe(e => Add(_fileSystem, e.EventArgs.FullPath))
                .DisposeWith(_disposables);

            Observable
                .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                    h => _watcher.Deleted += h,
                    h => _watcher.Deleted -= h)
                .Where(e => !_fileSystem.Path.HasExtension(e.EventArgs.FullPath) || IsFileRelevant(e.EventArgs.FullPath))
                .ObserveOnGui()
                .Subscribe(e => Remove(e.EventArgs.FullPath))
                .DisposeWith(_disposables);

            Observable
                .FromEventPattern<RenamedEventHandler, RenamedEventArgs>(
                    h => _watcher.Renamed += h,
                    h => _watcher.Renamed -= h)
                .ObserveOnGui()
                .Subscribe(e => {
                    Remove(e.EventArgs.OldFullPath);
                    Add(_fileSystem, e.EventArgs.FullPath);
                })
                .DisposeWith(_disposables);

            Observable
                .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                    h => _watcher.Changed += h,
                    h => _watcher.Changed -= h)
                .ObserveOnGui()
                .Subscribe(e => Change(e.EventArgs.FullPath))
                .DisposeWith(_disposables);
        }
    }

    private void Refresh() {
        if (Assets.Count > 0) {
            Assets.Clear();
        }

        var addedDirectories = new HashSet<string>(AssetCompare.PathComparer);
        IEnumerable<IAsset> assets = Array.Empty<IAsset>();
        if (!IsVirtual) {
            assets = assets.Concat(Directory.EnumerateDirectories()
                .Where(dir => !string.Equals(dir.FullName, _deleteDirectoryProvider.DeleteDirectory, AssetCompare.PathComparison))
                .Select(dirPath => addedDirectories.Add(dirPath.Name) ? new AssetDirectory(dirPath, _fileSystem, _deleteDirectoryProvider, _assetReferenceController, _assetTypeService, _archiveService) : null)
                .NotNull());
        }

        assets = assets.Concat(_archiveService.GetSubdirectories(Path)
            .Select(dirPath => _fileSystem.DirectoryInfo.New(dirPath))
            .Select(dirInfo => addedDirectories.Add(dirInfo.Name) ? new AssetDirectory(dirInfo, _fileSystem, _deleteDirectoryProvider, _assetReferenceController, _assetTypeService, _archiveService, true) : null)
            .NotNull());

        var addedFiles = new HashSet<string>();
        if (!IsVirtual) {
            assets = assets.Concat(Directory.EnumerateFiles()
                .Where(IsFileRelevant)
                .Select(file => {
                    var asset = FileToAsset(file.FullName);
                    return addedFiles.Add(file.Name) ? asset : null;
                })
                .NotNull());
        }

        assets = assets.Concat(_archiveService.GetFilesInDirectory(Directory.FullName)
            .Where(IsFileRelevant)
            .Select(file => {
                var asset = FileToAsset(file, true);
                return addedFiles.Add(_fileSystem.Path.GetFileName(file)) ? asset : null;
            })
            .NotNull());

        Assets.Edit(updater => {
            foreach (var asset in assets) {
                updater.AddOrUpdate(asset);
            }
        });
    }

    private AssetFile? FileToAsset(string file, bool isVirtual = false) {
        var assetLink = _assetTypeService.GetAssetLink(file);
        if (assetLink is null) return null;

        _assetReferenceController.GetReferencedAsset(assetLink, out var referencedAsset).DisposeWith(_disposables);
        var fileName = _fileSystem.Path.Combine(Path, _fileSystem.Path.GetFileName(assetLink.RawPath));
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
    /// <param name="absolutePath">absolute path of the asset to check for</param>
    /// <returns>true if the asset exists under this directory</returns>
    public bool Contains(string absolutePath) {
        return GetAssetFile(absolutePath) is not null;
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
                .FirstOrDefault(dir => string.Equals(_fileSystem.Path.GetFileName(dir.Path), directory, AssetCompare.PathComparison));

            if (currentDirectory is null) break;
        }

        return currentDirectory?.Children
            .OfType<AssetFile>()
            .FirstOrDefault(file => string.Equals(_fileSystem.Path.GetFileName(file.Path), directories[^1], AssetCompare.PathComparison));
    }

    /// <summary>
    /// Returns the asset file with the given file path.
    /// </summary>
    /// <param name="filePath">file path of the asset file</param>
    public AssetFile? this[string filePath] => GetAssetFile(filePath);
}
