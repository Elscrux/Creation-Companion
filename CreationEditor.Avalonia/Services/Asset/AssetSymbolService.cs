using CreationEditor.Services.Asset;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Asset;

public sealed class AssetSymbolService : IAssetSymbolService {
    private readonly Dictionary<string, Symbol> _fileExtensionSymbols = new();

    public AssetSymbolService
        (IAssetTypeProvider assetTypeProvider) {
        foreach (var extension in assetTypeProvider.Texture.FileExtensions) {
            _fileExtensionSymbols.Add(extension, Symbol.Image);
        }

        foreach (var extension in assetTypeProvider.Model.FileExtensions) {
            _fileExtensionSymbols.Add(extension, Symbol.Home);
        }

        foreach (var extension in assetTypeProvider.ScriptSource.FileExtensions) {
            _fileExtensionSymbols.Add(extension, Symbol.Code);
        }

        foreach (var extension in assetTypeProvider.Script.FileExtensions) {
            _fileExtensionSymbols.Add(extension, Symbol.Next);
        }

        foreach (var extension in assetTypeProvider.Sound.FileExtensions) {
            _fileExtensionSymbols.Add(extension, Symbol.Volume);
        }
    }

    public Symbol GetSymbol(string fileExtension) {
        return _fileExtensionSymbols.GetValueOrDefault(fileExtension, Symbol.OpenFile);
    }
}
