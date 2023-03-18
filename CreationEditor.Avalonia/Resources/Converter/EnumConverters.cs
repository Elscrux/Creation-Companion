using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Converter;

public static class EnumConverters {
    public new static readonly FuncValueConverter<Enum, string> ToString
        = new(e => e?.ToString() ?? string.Empty);
    
    public static readonly FuncValueConverter<Enum, string> ToStringWithSpaces
        = new(e => e == null
            ? string.Empty
            : string.Concat(e.ToString().Select((c, i) => i != 0 && char.IsUpper(c) ? " " + c : c.ToString())));
}
