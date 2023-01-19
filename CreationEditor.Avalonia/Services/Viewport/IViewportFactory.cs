using Avalonia.Controls;
namespace CreationEditor.Avalonia.Services.Viewport; 

public interface IViewportFactory {
    public bool IsMultiInstanceCapable { get; }

    public Control CreateViewport();
}
