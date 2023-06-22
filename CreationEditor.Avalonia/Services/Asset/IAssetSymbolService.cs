using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Asset;

public interface IAssetSymbolService {
    Symbol GetSymbol(string fileExtension);
}
