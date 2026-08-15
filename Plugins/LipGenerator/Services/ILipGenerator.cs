using Mutagen.Bethesda.Strings;
namespace LipGenerator.Services;

public interface ILipGeneratorArgs;

public interface ILipGenerator {
    static abstract IReadOnlyList<Language> SupportedLanguages { get; }

    bool GenerateLip(string wavPath, string text);
}
