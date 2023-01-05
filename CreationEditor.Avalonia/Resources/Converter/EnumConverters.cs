using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Converter;

public static class EnumConverters {
    public new static readonly FuncValueConverter<Enum, string> ToString
        = new(e => e?.ToString() ?? string.Empty);
}
