using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Controls;
using CreationEditor.Avalonia.Views.Viewport;
using Mutagen.Bethesda.Environments.DI;
using Serilog;
namespace CreationEditor.Avalonia.Services.Viewport.BSE;

public sealed class BSEViewportFactory : IViewportFactory {
    private readonly ILogger _logger;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IFileSystem _fileSystem;

    public bool IsMultiInstanceCapable => false;

    private readonly Subject<Unit> _viewportInitialized = new();
    public IObservable<Unit> ViewportInitialized => _viewportInitialized;

    public BSEViewportFactory(
        ILogger logger,
        IDataDirectoryProvider dataDirectoryProvider,
        IFileSystem fileSystem) {
        _logger = logger;
        _dataDirectoryProvider = dataDirectoryProvider;
        _fileSystem = fileSystem;
    }

    public Control CreateViewport() {
        var assetDirectory = _dataDirectoryProvider.Path;
        var bsaFileNames = _fileSystem.Directory
            .EnumerateFiles(assetDirectory, "*.bsa")
            .Select(path => _fileSystem.Path.GetFileName(path))
            // .OrderBy() todo order by load order priority (+ exclude non-loaded BSAs? would need ini parsing to check which inis are always loaded)
            .ToArray();

        _logger.Here().Information("Initializing viewport with data directory {AssetDirectory}", assetDirectory);

        if (bsaFileNames.Length > 0) {
            _logger.Here().Debug(
                "Loading BSAs: {FileNames}", string.Join(", ", bsaFileNames));
        } else {
            _logger.Here().Warning("No BSAs detected in {AssetDirectory}", assetDirectory);
        }

        var viewportBSE = new ViewportBSE(
            _logger,
            assetDirectory,
            bsaFileNames
        );

        // Viewport creation blocks until the viewport is fully initialized, so we can safely fire the event here
        _viewportInitialized.OnNext(Unit.Default);

        return viewportBSE;
    }
}
