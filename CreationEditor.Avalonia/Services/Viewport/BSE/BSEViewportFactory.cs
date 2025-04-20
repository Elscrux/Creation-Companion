using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Controls;
using CreationEditor.Avalonia.Views.Viewport;
using CreationEditor.Services.Archive;
using Mutagen.Bethesda.Environments.DI;
using Noggog;
using Serilog;
using static ProjectBSE.Interop.Interop;
namespace CreationEditor.Avalonia.Services.Viewport.BSE;

public sealed class BSEViewportFactory(
    ILogger logger,
    IDataDirectoryProvider dataDirectoryProvider,
    IArchiveService archiveService)
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
            var viewportHost = new ViewportHost(mainWindowHandle, "Viewport");
            _viewportInitialized.OnNext(Unit.Default);
            return viewportHost;
        });
    }

    private void StartupEditor() {
        var assetDirectory = dataDirectoryProvider.Path;
        var archiveLoadOrder = archiveService.GetArchiveLoadOrder().ToArray();

        logger.Here().Information("Initializing viewport with data directory {AssetDirectory}", assetDirectory);

        if (archiveLoadOrder.Length > 0) {
            logger.Here().Debug(
                "Loading archives: {FileNames}",
                string.Join(", ", archiveLoadOrder));
        } else {
            logger.Here().Warning("No archives detected in {AssetDirectory}", assetDirectory);
        }

        var initConfig = new InitConfig {
            Version = CurrentVersion,
            AssetDirectory = assetDirectory,
        };

        var code = InitTGEditor(initConfig, archiveLoadOrder);
        logger.Here().Information("Closed viewport with code {Code}", code);
    }
}
