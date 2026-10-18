using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Avalonia.Views.Asset.Browser;
using CreationEditor.Services.DataSource;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Docking.Definitions;

/// <summary>
/// Browse assets. The optional parameter is an <see cref="IDataSourceLink"/> used as the base path for browsing.
/// </summary>
public sealed class AssetBrowserDockable(
    Func<IAssetBrowserVM> assetBrowserVMFactory)
    : IDockableDefinition {
    public string Type => DockableTypes.AssetBrowser;
    public string Header => "Asset Browser";
    public object Icon => FASymbol.Folder;
    public string Category => "View";
    public KeyGesture HotKey => new(Key.A, KeyModifiers.Control | KeyModifiers.Alt);
    public double? Size => 400;
    public bool CanClose => true;
    public DockMode DockMode => DockMode.Side;
    public Dock Dock => Dock.Left;
    public GridLength GridSize => new(1, GridUnitType.Star);

    public async Task<Control?> CreateControl(object? parameter = null) {
        var assetBrowserVM = await Task.Run(assetBrowserVMFactory);
        return new AssetBrowser(assetBrowserVM, parameter as IDataSourceLink);
    }
}
