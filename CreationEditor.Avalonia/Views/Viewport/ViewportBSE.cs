using System.Diagnostics;
using Avalonia.Controls.Presenters;
using Avalonia.Threading;
using CreationEditor.Avalonia.Services.Viewport.BSE;
using Noggog;
using Serilog;
namespace CreationEditor.Avalonia.Views.Viewport;

public sealed class ViewportBSE : ContentPresenter {
    private const string ViewportProcessName = "BSE";
    private const int ViewportEmbeddingAttempts = 10;

    public ViewportBSE(ILogger logger, string assetDirectory, string[] bsaFileNames) {
        Task.Run(() => {
            var initConfig = new Interop.InitConfig {
                Version = 2,
                AssetDirectory = assetDirectory,
                SizeOfWindowHandles = 0,
                WindowHandles = Array.Empty<nint>(),
            };

            try {
                var code = Interop.initTGEditor(initConfig, bsaFileNames, (ulong) bsaFileNames.Length);
                logger.Here().Information("Closed viewport with code {Code}", code);
            } catch (Exception e) {
                logger.Here().Error("Initializing viewport failed: {Message}", e.ToString());

                // Invalidate the viewport
                Dispatcher.UIThread.Post(() => Content = null);
            }
        });

        Interop.addLoadCallback((callbackCount, callbackLoads) => {
            // logger.Here().Verbose("Loaded {Count} references: {Refs}", callbackCount, string.Join(", ", callbackLoads));
        });

        Interop.waitFinishedInit();

        // Give it a little more time to finish
        Thread.Sleep(250);

        var process = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)
            .NotNull()
            .FirstOrDefault(p => p.MainWindowTitle == ViewportProcessName);

        if (process is not null) Content = new ViewportHost(process);
    }
}
