using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Asset;

public sealed class AssetSymbolService : IAssetSymbolService {
    private readonly Dictionary<string, Symbol> _fileExtensionSymbols;

    public AssetSymbolService(
        IAssetTypeProvider assetTypeProvider,
        IArchiveService archiveService) {
        _fileExtensionSymbols= new Dictionary<string, Symbol> {
            { ".esp", Symbol.Save },
            { ".esm", Symbol.Save },
            { ".esl", Symbol.Save },
            { archiveService.GetExtension(), Symbol.Library },
        };

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

        foreach (var extension in assetTypeProvider.Music.FileExtensions.Concat(assetTypeProvider.Sound.FileExtensions).Distinct()) {
            _fileExtensionSymbols.Add(extension, Symbol.Volume);
        }
    }

    public Symbol GetSymbol(string fileExtension) {
        return _fileExtensionSymbols.GetValueOrDefault(fileExtension, Symbol.OpenFile);
    }
}
