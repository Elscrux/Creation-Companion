using System.Diagnostics;
using Avalonia.Controls.Presenters;
using CreationEditor.Avalonia.Services.Viewport;
using CreationEditor.Services.Environment;
using Noggog;
namespace CreationEditor.Avalonia.Views.Viewport;

public class RenderView : ContentPresenter {
    public RenderView(IEnvironmentContext environmentContext) {
        Interop.Start(environmentContext);
        
        var process = Process.GetProcessesByName("CreationEditor.Skyrim.Avalonia")
            .NotNull()
            .FirstOrDefault(p => p.MainWindowTitle == "BSE");

        if (process != null) Content = new RenderViewHost(process);
    }
}
