using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Controls;
using CreationEditor.Avalonia.Views.Viewport;
using Mutagen.Bethesda.Environments.DI;
using Noggog;
using Serilog;
using static ProjectBSE.Interop.Interop;
namespace CreationEditor.Avalonia.Services.Viewport.BSE;

public sealed class BSEViewportFactory(
    ILogger logger,
    IDataDirectoryProvider dataDirectoryProvider,
    IFileSystem fileSystem)
    : IViewportFactory {

    public bool IsMultiInstanceCapable => false;

    private readonly Subject<Unit> _viewportInitialized = new();
    public IObservable<Unit> ViewportInitialized => _viewportInitialized;

    public Task<Func<Control>> CreateViewport() {
        // Run startup logic
        Task.Run(StartupEditor)
            .FireAndForget(e => logger.Here().Error("Initializing viewport failed: {Message}", e.ToString()));

        // Wait for viewport internal startup logic to finish
        WaitFinishedInit();

        // Capture the viewport process and embed it
        var mainWindowHandle = GetMainWindowHandle();

        return Task.FromResult<Func<Control>>(() => {
            var viewportHost = new WindowHandleHost(mainWindowHandle, "Viewport");
            _viewportInitialized.OnNext(Unit.Default);
            return viewportHost;
        });
    }

    private void StartupEditor() {
        var assetDirectory = dataDirectoryProvider.Path;
        var bsaFileNames = fileSystem.Directory
            .EnumerateFiles(assetDirectory, "*.bsa")
            .Select(path => fileSystem.Path.GetFileName(path))
            // .OrderBy() todo order by load order priority (+ exclude non-loaded BSAs? would need ini parsing to check which inis are always loaded)
            .ToArray();

        logger.Here().Information("Initializing viewport with data directory {AssetDirectory}", assetDirectory);

        if (bsaFileNames.Length > 0) {
            logger.Here().Debug(
                "Loading BSAs: {FileNames}", string.Join(", ", bsaFileNames));
        } else {
            logger.Here().Warning("No BSAs detected in {AssetDirectory}", assetDirectory);
        }

        var initConfig = new InitConfig {
            Version = CurrentVersion,
            AssetDirectory = assetDirectory,
        };

        var code = InitTGEditor(initConfig, bsaFileNames, (ulong) bsaFileNames.Length);
        logger.Here().Information("Closed viewport with code {Code}", code);
    }
}
