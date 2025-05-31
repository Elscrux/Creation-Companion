using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Avalonia.Services.Asset;

public interface IAssetIconService {
    /// <summary>
    /// Creates a control that displays the icon for the given file extension.
    /// </summary>
    /// <param name="assetType">Asset type to get the icon for</param>
    /// <returns>A control that displays the icon for the file extension</returns>
    FAIconElement GetIcon(IAssetType? assetType);

    /// <summary>
    /// Creates a control that displays the icon for the given symbol.
    /// </summary>
    /// <param name="symbol">Symbol to display</param>
    /// <param name="tooltip">Optional tooltip to display when hovering over the icon</param>
    /// <returns>A control that displays the icon for the symbol</returns>
    FAIconElement GetIcon(Symbol symbol, string? tooltip = null);

    /// <summary>
    /// Creates a control that displays the icon for the given symbol.
    /// </summary>
    /// <param name="extension">File extension to get the icon for</param>
    /// <param name="tooltip">Optional tooltip to display when hovering over the icon</param>
    /// <returns>A control that displays the icon for the symbol</returns>
    FAIconElement GetIcon(string extension, string? tooltip = null);
}
