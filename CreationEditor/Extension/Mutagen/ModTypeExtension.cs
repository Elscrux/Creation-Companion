using Mutagen.Bethesda.Plugins;
namespace CreationEditor;

public static class ModTypeExtension {
    public const string MasterFileExtension = ".esm";
    public const string PluginFileExtension = ".esp";
    public const string LightPluginFileExtension = ".esl";

    extension(ModType modType) {
        public string ToFileExtension() => modType switch {
            ModType.Master => MasterFileExtension,
            ModType.Light => LightPluginFileExtension,
            ModType.Plugin => PluginFileExtension,
            _ => throw new ArgumentOutOfRangeException(nameof(modType), modType, null),
        };
    }

    public static ModType FromFileExtension(string name) => name switch {
        MasterFileExtension => ModType.Master,
        LightPluginFileExtension => ModType.Light,
        PluginFileExtension => ModType.Plugin,
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
    };
}
