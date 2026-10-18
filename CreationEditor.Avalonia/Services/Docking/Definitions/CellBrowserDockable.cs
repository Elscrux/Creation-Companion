using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Record.Browser;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Docking.Definitions;

public sealed class CellBrowserDockable(
    ICellBrowserFactory cellBrowserFactory)
    : IDockableDefinition {
    public string Type => DockableTypes.CellBrowser;
    public string Header => "Cell Browser";
    public object Icon => FASymbol.Image;
    public string Category => "View";
    public KeyGesture HotKey => new(Key.C, KeyModifiers.Control | KeyModifiers.Alt);
    public double? Size => 400;
    public bool CanClose => true;
    public DockMode DockMode => DockMode.Side;
    public Dock Dock => Dock.Right;
    public GridLength GridSize => new(1, GridUnitType.Star);

    public Task<Control?> CreateControl(object? parameter = null) => Task.FromResult<Control?>(cellBrowserFactory.GetBrowser());
}
