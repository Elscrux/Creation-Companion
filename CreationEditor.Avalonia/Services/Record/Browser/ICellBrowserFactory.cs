using Avalonia.Controls;
namespace CreationEditor.Avalonia.Services.Record.Browser;

public interface ICellBrowserFactory {
    /// <summary>
    /// Create a cell browser
    /// </summary>
    /// <returns>Created cells browser</returns>
    Control GetBrowser();
}
