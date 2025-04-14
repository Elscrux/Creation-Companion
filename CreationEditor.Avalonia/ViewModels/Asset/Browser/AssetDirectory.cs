// using System.IO.Abstractions;
// using CreationEditor.Services.Archive;
// using CreationEditor.Services.DataSource;
// using CreationEditor.Services.Mutagen.References.Asset;
// using CreationEditor.Services.Mutagen.References.Asset.Controller;
// using DynamicData;
// using Mutagen.Bethesda.Assets;
// using Mutagen.Bethesda.Plugins.Assets;
// using Mutagen.Bethesda.Plugins.Exceptions;
// using Noggog;
// using Serilog;
// namespace CreationEditor.Services.Asset;
//
// // TODO is this the right way to handle directories? Is this not more of a VM and the model should be simpler? are all features always needed
// public sealed class AssetDirectory : IAsset {
//     private readonly DisposableBucket _disposables = new();
//
//     private readonly ILogger _logger;
//     private readonly IAssetTypeService _assetTypeService;
//     private readonly IArchiveService _archiveService;
//     private readonly IDataSourceWatcher _dataSourceWatcher;
//     private readonly IAssetReferenceController _assetReferenceController;
//
//     public FileSystemLink Link { get; }
//
//     public bool IsDirectory => true;
//     public bool HasChildren {
//         get {
//             if (!_loadedAssets) LoadAssets();
//
//             return Assets.Count > 0;
//         }
//     }
//     public bool IsVirtual { get; }
//
//     public IEnumerable<IAsset> Children {
//         get {
//             if (!_loadedAssets) LoadAssets();
//
//             return Assets.Items;
//         }
//     }
//
//     private bool _loadedAssets;
//     public SourceCache<IAsset, string> Assets { get; } = new(a => a.Link.DataRelativePath.Path);
//
//     public AssetDirectory(
//         FileSystemLink link,
//         ILogger logger,
//         IDataSourceWatcher dataSourceWatcher,
//         IAssetReferenceController assetReferenceController,
//         IAssetTypeService assetTypeService,
//         IArchiveService archiveService,
//         bool isVirtual) {
//         _logger = logger;
//         _assetTypeService = assetTypeService;
//         _archiveService = archiveService;
//         _dataSourceWatcher = dataSourceWatcher;
//         _assetReferenceController = assetReferenceController;
//         Link = link;
//         IsVirtual = isVirtual;
//     }
//
//     public AssetDirectory(
//         FileSystemLink link,
//         ILogger logger,
//         IDataSourceWatcher dataSourceWatcher,
//         IAssetReferenceController assetReferenceController,
//         IAssetTypeService assetTypeService,
//         IArchiveService archiveService)
//         : this(
//             link,
//             logger,
//             dataSourceWatcher,
//             assetReferenceController,
//             assetTypeService,
//             archiveService,
//             link.Exists()) {}
//
//     // private void Add(string path) {
//     //     try {
//     //         var directoryInfo = Link.FileSystem.DirectoryInfo.New(path);
//     //         if ((directoryInfo.Attributes & FileAttributes.Directory) == 0) {
//     //             var asset = FileToAsset(path);
//     //             if (asset is null) return;
//     //
//     //             asset.DisposeWith(_disposables);
//     //
//     //             _assetReferenceController.RegisterCreation(asset);
//     //             Assets.AddOrUpdate(asset);
//     //         } else {
//     //             var assetDirectory = new AssetDirectory(
//     //                 link,
//     //                 _logger,
//     //                 _dataDirectoryService,
//     //                 _assetReferenceController,
//     //                 _assetTypeService,
//     //                 _archiveService,
//     //                 false);
//     //             assetDirectory.DisposeWith(_disposables);
//     //             Assets.AddOrUpdate(assetDirectory);
//     //         }
//     //     } catch (Exception) {
//     //         Refresh();
//     //     }
//     // }
//
//     // private void Remove(string path) {
//     //     try {
//     //         var fileName = Link.FileSystem.Path.GetFileName(path);
//     //         using var asset = Assets.Items.FirstOrDefault(asset => {
//     //             var assetName = asset.Link.Name;
//     //             return string.Equals(assetName, fileName, DataRelativePath.PathComparison);
//     //         });
//     //         if (asset is null) return;
//     //
//     //         Assets.Remove(asset);
//     //         if (asset is AssetFile file) {
//     //             _assetReferenceController.RegisterDeletion(file);
//     //         } else {
//     //             var childFiles = asset.Children
//     //                 .GetAllChildren(x => x.Children)
//     //                 .OfType<AssetFile>();
//     //
//     //             foreach (var childFile in childFiles) {
//     //                 _assetReferenceController.RegisterDeletion(childFile);
//     //             }
//     //         }
//     //     } catch (Exception) {
//     //         Refresh();
//     //     }
//     // }
//
//     // private void Change(string path) {
//     //     try {
//     //         var fileName = Link.FileSystem.Path.GetFileName(path);
//     //         var asset = Assets.Items.FirstOrDefault(asset => {
//     //             var assetName = asset.Link.Name;
//     //             return string.Equals(assetName, fileName, DataRelativePath.PathComparison);
//     //         });
//     //         if (asset is not AssetFile file) return;
//     //
//     //         var registerUpdate = _assetReferenceController.RegisterUpdate(file);
//     //         registerUpdate(file);
//     //     } catch (Exception) {
//     //         Refresh();
//     //     }
//     // }
//
//     private bool IsFileRelevant(IFileInfo file) {
//         return _assetTypeService.FileExtensions.Contains(file.Extension);
//     }
//
//     private bool IsFileRelevant(string file) {
//         var extension = Link.FileSystem.Path.GetExtension(file);
//
//         return _assetTypeService.FileExtensions.Contains(extension);
//     }
//
//     public void Dispose() => _disposables.Dispose();
//
//     public void LoadAssets() {
//         if (_loadedAssets) return;
//
//         _loadedAssets = true;
//
//         Refresh();
//
//         // TODO REIMPLEMENT
//         // if (!IsVirtual) {
//         //     // potential performance issue - every loaded directory will check if a file change is relevant
//         //     // instead, it might be best to subscribe just for changes in the current directory
//         //     // would require a change in the data directory service api
//         //     _dataDirectoryService.Created
//         //         .Where(e => Link.Equals(Link.FileSystem.Path.GetDirectoryName(e.FullPath), DataRelativePath.PathComparison))
//         //         .ObserveOnGui()
//         //         .Subscribe(e => Add(e.FullPath))
//         //         .DisposeWith(_disposables);
//         //
//         //     _dataDirectoryService.Deleted
//         //         .Where(e => Link.Equals(Link.FileSystem.Path.GetDirectoryName(e.FullPath), DataRelativePath.PathComparison))
//         //         .ObserveOnGui()
//         //         .Subscribe(e => {
//         //             var link = new FileSystemLink(Link.DataSource, e.)
//         //             Remove(e.FullPath);
//         //         })
//         //         .DisposeWith(_disposables);
//         //
//         //     _dataDirectoryService.Renamed
//         //         .Where(e => Link.Equals(Link.FileSystem.Path.GetDirectoryName(e.FullPath), DataRelativePath.PathComparison))
//         //         .ObserveOnGui()
//         //         .Subscribe(e => {
//         //             Remove(e.OldFullPath);
//         //             Add(e.FullPath);
//         //         })
//         //         .DisposeWith(_disposables);
//         //
//         //     _dataDirectoryService.Changed
//         //         .Where(e => Link.Equals(Link.FileSystem.Path.GetDirectoryName(e.FullPath), DataRelativePath.PathComparison))
//         //         .ObserveOnGui()
//         //         .Subscribe(e => Change(e.FullPath))
//         //         .DisposeWith(_disposables);
//         // }
//     }
//
//     private void Refresh() {
//         var addedDirectories = new HashSet<string>(DataRelativePath.PathComparer);
//         IEnumerable<IAsset> assets = [];
//         if (!IsVirtual) {
//             assets = assets.Concat(Link.FileSystem.Directory.EnumerateDirectories(Link.FullPath)
//                 .Select(dirPath => addedDirectories.Add(dirPath.Name)
//                     ? new AssetDirectory(
//                         dirPath,
//                         _logger,
//                         _dataSourceWatcher,
//                         _assetReferenceController,
//                         _assetTypeService,
//                         _archiveService,
//                         false)
//                     : null)
//                 .WhereNotNull());
//         }
//
//         assets = assets.Concat(_archiveService.GetSubdirectories(Link)
//             .Select(dirPath => Link.FileSystem.DirectoryInfo.New(dirPath))
//             .Select(dirInfo => addedDirectories.Add(dirInfo.Name)
//                 ? new AssetDirectory(
//                     dirInfo,
//                     _logger,
//                     _dataSourceWatcher,
//                     _assetReferenceController,
//                     _assetTypeService,
//                     _archiveService,
//                     true)
//                 : null)
//             .WhereNotNull());
//
//         var addedFiles = new HashSet<string>();
//         if (!IsVirtual) {
//             assets = assets.Concat(Directory.EnumerateFiles()
//                 .Where(IsFileRelevant)
//                 .Select(file => {
//                     var asset = FileToAsset(file.FullName);
//                     return addedFiles.Add(file.Name) ? asset : null;
//                 })
//                 .WhereNotNull());
//         }
//
//         assets = _archiveService.GetFilesInDirectory(Directory.FullName)
//             .Where(IsFileRelevant)
//             .Select(file => {
//                 var asset = FileToAsset(file, true);
//                 return addedFiles.Add(Link.FileSystem.Path.GetFileName(file)) ? asset : null;
//             })
//             .WhereNotNull()
//             .Concat(assets)
//             .ToList();
//
//         Assets.Edit(updater => {
//             updater.Clear();
//             foreach (var asset in assets) {
//                 updater.AddOrUpdate(asset);
//             }
//         });
//     }
//
//     private AssetFile? FileToAsset(string file, bool isVirtual = false) {
//         IAssetLink? assetLink;
//         try {
//             assetLink = _assetTypeService.GetAssetLink(file);
//         } catch (AssetPathMisalignedException e) {
//             _logger.Here().Warning(e, "Failed to parse asset path {Path}: {Exception}", file, e.Message);
//             return null;
//         }
//         if (assetLink is null) return null;
//
//         _assetReferenceController.GetReferencedAsset(assetLink, out var referencedAsset).DisposeWith(_disposables);
//         var fileName = Link.FileSystem.Path.Combine(Link.FullPath, Link.FileSystem.Path.GetFileName(assetLink.DataRelativePath.Path));
//         return new AssetFile(fileName, referencedAsset, isVirtual);
//     }
//
//     public IEnumerable<IReferencedAsset> GetReferencedAssets() {
//         return this
//             .GetAllChildren<IAsset>(a => a.Children)
//             .OfType<AssetFile>()
//             .Select(file => file.ReferencedAsset);
//     }
//
//     /// <summary>
//     /// Checks if the given asset exists in this directory or any of its subdirectories.
//     /// </summary>
//     /// <param name="fullPath">Full path of the asset to check for</param>
//     /// <returns>True if the asset exists under this directory</returns>
//     public bool Contains(FileSystemLink fullPath) {
//         return GetAssetFile(fullPath) is not null;
//     }
//
//     /// <summary>
//     /// Returns the asset file with the given file path.
//     /// </summary>
//     /// <param name="fileLink">File link of the asset file</param>
//     /// <returns>Asset file with the given file path</returns>
//     public AssetFile? GetAssetFile(FileSystemLink fileLink) {
//         if (!Link.Contains(fileLink)) return null;
//
//         var relativePath = Link.FileSystem.Path.GetRelativePath(Link.FullPath, fileLink.FullPath);
//         if (relativePath.IsNullOrWhitespace()) return null;
//
//         var currentDirectory = this;
//         var directories = relativePath.Split(Link.FileSystem.Path.DirectorySeparatorChar, Link.FileSystem.Path.AltDirectorySeparatorChar).ToArray();
//
//         foreach (var directory in directories.SkipLast(1)) {
//             currentDirectory = currentDirectory.Children
//                 .OfType<AssetDirectory>()
//                 .FirstOrDefault(dir => string.Equals(dir.Link.Name, directory, DataRelativePath.PathComparison));
//
//             if (currentDirectory is null) break;
//         }
//
//         return currentDirectory?.Children
//             .OfType<AssetFile>()
//             .FirstOrDefault(file => string.Equals(file.Link.Name, directories[^1], DataRelativePath.PathComparison));
//     }
//
//     /// <summary>
//     /// Returns the asset file with the given file path.
//     /// </summary>
//     /// <param name="filePath">file path of the asset file</param>
//     public AssetFile? this[FileSystemLink filePath] => GetAssetFile(filePath);
// }
