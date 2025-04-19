using Avalonia.Media;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Avalonia.Services.Avalonia.Font;

public class GameFontLoader : IGameFontLoader {
    private readonly Uri _fontsDirectoryUri = new(new Uri(AppContext.BaseDirectory), "./Resources/Fonts");
    private readonly string _fontCollectionName;

    public GameFontLoader(IGameReleaseContext gameReleaseContext) {
        _fontCollectionName = $"fonts:{gameReleaseContext.Release}";

        Init();
    }

    private void Init() {
        // todo automatically extract ttf fonts from swf files as referenced in (fontProvider.FontLibraries) and copy them into ./Resources/Fonts
        FontManager.Current.AddFontCollection(new FileSystemFontCollection(new Uri(_fontCollectionName), _fontsDirectoryUri));
    }

    public string GetFontName(string fontId) => $"{_fontCollectionName}#{fontId}";
}
