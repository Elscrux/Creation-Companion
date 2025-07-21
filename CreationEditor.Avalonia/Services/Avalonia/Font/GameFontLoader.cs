using System.IO.Abstractions;
using Avalonia.Media;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Avalonia.Services.Avalonia.Font;

public class GameFontLoader : IGameFontLoader {
    private readonly IFileSystem _fileSystem;
    private readonly Uri _fontsDirectoryUri = new(new Uri(AppContext.BaseDirectory), "./Resources/Fonts");
    private readonly string _fontCollectionName;

    public GameFontLoader(
        IFileSystem fileSystem,
        IGameReleaseContext gameReleaseContext) {
        _fileSystem = fileSystem;
        _fontCollectionName = $"fonts:{gameReleaseContext.Release}";

        Init();
    }

    private void Init() {
        // todo automatically extract ttf fonts from swf files as referenced in (fontProvider.FontLibraries) and copy them into ./Resources/Fonts

        // Create directory
        if (!_fileSystem.Directory.Exists(_fontsDirectoryUri.LocalPath)) {
            _fileSystem.Directory.CreateDirectory(_fontsDirectoryUri.LocalPath);
        }

        FontManager.Current.AddFontCollection(new FileSystemFontCollection(new Uri(_fontCollectionName), _fontsDirectoryUri));
    }

    public string GetFontName(string fontId) => $"{_fontCollectionName}#{fontId}";
}
