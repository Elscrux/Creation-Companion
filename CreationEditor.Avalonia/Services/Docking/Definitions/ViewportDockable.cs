using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Viewport;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Docking.Definitions;

public sealed class ViewportDockable(
    IViewportFactory viewportFactory)
    : IDockableDefinition {
    private bool _viewportCreated;

    public string Type => DockableTypes.Viewport;
    public string Header => "Viewport";
    public object Icon => FASymbol.Pictures;
    public string Category => "View";
    public KeyGesture? HotKey => null;
    public double? Size => null;
    public bool CanClose => viewportFactory.IsMultiInstanceCapable;
    public DockMode DockMode => DockMode.Layout;
    public Dock Dock => Dock.Right;
    public GridLength GridSize => new(3, GridUnitType.Star);

    public async Task<Control?> CreateControl(object? parameter = null) {
        if (_viewportCreated && !viewportFactory.IsMultiInstanceCapable) return null;

        var getControl = await viewportFactory.CreateViewport();
        _viewportCreated = true;

        return await Dispatcher.UIThread.InvokeAsync(getControl);
    }
}
