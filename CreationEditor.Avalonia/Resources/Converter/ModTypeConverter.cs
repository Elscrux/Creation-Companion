using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Converter;

public static class ModTypeConverter {
    private const string MasterFileExtension = ".esm";
    private const string PluginFileExtension = ".esp";
    private const string LightPluginFileExtension = ".esl";

    public static readonly ExtendedFuncValueConverter<ModType, string, object> ToFileExtension
        = new((modType, _) => modType switch {
            ModType.Master => MasterFileExtension,
            ModType.Light => LightPluginFileExtension,
            ModType.Plugin => PluginFileExtension,
            _ => throw new ArgumentOutOfRangeException(nameof(modType), modType, null)
        }, (name, _) => name switch {
            MasterFileExtension => ModType.Master,
            LightPluginFileExtension => ModType.Light,
            PluginFileExtension => ModType.Plugin,
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
        });
}
