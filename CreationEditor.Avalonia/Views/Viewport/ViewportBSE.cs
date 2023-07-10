using System.Diagnostics;
using Avalonia.Controls.Presenters;
using Avalonia.Threading;
using CreationEditor.Avalonia.Services.Viewport.BSE;
using Noggog;
using Serilog;
namespace CreationEditor.Avalonia.Views.Viewport;

public sealed class ViewportBSE : ContentPresenter {
    private const string ViewportProcessName = "BSE";
    private const int ViewportEmbeddingAttempts = 50;

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

        Interop.waitFinishedInit();

        Process? process = null;
        for (var i = 0; i < ViewportEmbeddingAttempts; i++) {
            process = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)
                .NotNull()
                .FirstOrDefault(p => p.MainWindowTitle == ViewportProcessName);

            if (process is null) {
                logger.Here().Verbose("Waiting for viewport to start...");
                Thread.Sleep(100);
            } else {
                logger.Here().Verbose("Viewport started, now embed it into the application");
                break;
            }
        }

        if (process is null) {
            logger.Here().Warning("Failed to embed viewport after {Count} attempts", ViewportEmbeddingAttempts);
        } else {
            Content = new ViewportHost(process);
        }
    }
}
