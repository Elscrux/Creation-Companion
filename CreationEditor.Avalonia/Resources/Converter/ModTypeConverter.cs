using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Converter;

public static class ModTypeConverter {
    public static readonly ExtendedFuncValueConverter<ModType, string, object> ToFileExtension
        = new((modType, _) => modType.ToFileExtension(),
            (name, _) => name is null
                ? throw new ArgumentNullException(nameof(name))
                : ModTypeExtension.FromFileExtension(name));
}
