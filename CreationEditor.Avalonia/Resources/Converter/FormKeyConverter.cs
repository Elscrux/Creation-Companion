using Avalonia.Data.Converters;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Converter;

public static class FormKeyConverter {
    public static readonly IValueConverter IsNull =
        new FuncValueConverter<FormKey, bool>(x => x == FormKey.Null);

    public static readonly IValueConverter IsNotNull =
        new FuncValueConverter<FormKey, bool>(x => x != FormKey.Null);
}
