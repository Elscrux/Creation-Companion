using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Asset;

public interface IAssetSymbolService {
    /// <summary>
    /// Maps file extensions to a symbol.
    /// </summary>
    /// <param name="fileExtension">File extension starting with a dot</param>
    /// <returns>A symbol matching the file extension</returns>
    Symbol GetSymbol(string fileExtension);
}
