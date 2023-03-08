using System.Diagnostics;
using Avalonia.Controls.Presenters;
using CreationEditor.Avalonia.Services.Viewport.BSE;
using Noggog;
namespace CreationEditor.Avalonia.Views.Viewport;

public class ViewportBSE : ContentPresenter {
    private const string ViewportProcessName = "BSE";

    public ViewportBSE(string assetDirectory) {
        Task.Run(() => {
            Interop.initTGEditor(new Interop.InitConfig {
                Version = 1,
                AssetDirectory = assetDirectory
            });
        });

        Interop.addLoadCallback((callbackCount, callbackLoads) => {
            Console.WriteLine($"CALLBACK: {callbackCount}");
            if (callbackCount == 0) return;

            foreach (var load in callbackLoads) {
                Console.WriteLine($"CALLBACK: {load.ToString()}");
            }
        });

        Interop.waitFinishedInit();

        // Give it a little more time to finish
        Thread.Sleep(250);

        var process = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)
            .NotNull()
            .FirstOrDefault(p => p.MainWindowTitle == ViewportProcessName);

        if (process != null) Content = new ViewportHost(process);
    }
}
