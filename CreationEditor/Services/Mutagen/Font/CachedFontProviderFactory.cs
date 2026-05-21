using Mutagen.Bethesda.Fonts;
using Mutagen.Bethesda.Fonts.DI;
using Mutagen.Bethesda.Strings;
namespace CreationEditor.Services.Mutagen.Font;

public class CachedFontProviderFactory(FontProviderFactory fontProviderFactory) : IFontProviderFactory {
    private readonly Dictionary<Language, IFontProvider> _cache = new();

    public IFontProvider Create(Language language) {
        if (_cache.TryGetValue(language, out var provider)) return provider;

        provider = fontProviderFactory.Create(language);
        _cache[language] = provider;
        return provider;
    }
}
