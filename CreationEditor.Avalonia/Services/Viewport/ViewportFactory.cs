using Avalonia.Controls;
using CreationEditor.Avalonia.Views.Viewport;
using CreationEditor.Services.Environment;
namespace CreationEditor.Avalonia.Services.Viewport; 

public class BSEViewportFactory : IViewportFactory {
    private readonly IEnvironmentContext _environmentContext;
    
    public bool IsMultiInstanceCapable => false;

    public BSEViewportFactory(
        IEnvironmentContext environmentContext) {
        _environmentContext = environmentContext;
    }
    
    public Control CreateViewport() {
        return new ViewportBSE(
            @"E:\TES\Skyrim\vanilla-files\"//_environmentContext.DataDirectoryProvider.Path
            );
    }
}
