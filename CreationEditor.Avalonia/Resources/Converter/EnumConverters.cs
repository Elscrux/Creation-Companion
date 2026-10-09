using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Converter;

public static class EnumConverters {
    /// <summary>
    /// Checks if two enum values are equal.
    /// </summary>
    public new static readonly ExtendedFuncValueConverter<Enum, bool, Enum> Equals
        = new((e, par) => Convert.ToInt32(e) == Convert.ToInt32(par));

    /// <summary>
    /// Checks if two enum values are not equal.
    /// </summary>
    public static readonly ExtendedFuncValueConverter<Enum, bool, Enum> NotEquals
        = new((e, par) => Convert.ToInt32(e) != Convert.ToInt32(par));

    /// <summary>
    /// Converts an enum to a string using its ToString() method.
    /// </summary>
    public new static readonly FuncValueConverter<Enum, string> ToString
        = new(e => e?.ToString() ?? string.Empty);

    /// <summary>
    /// Converts an enum to a string with spaces before capital letters, except between two
    /// consecutive capital letters (e.g. TestDDS -> "Test DDS").
    /// </summary>
    public static readonly FuncValueConverter<Enum, string> ToStringWithSpaces
        = new(e => e is null
            ? string.Empty
            : string.Concat(e
                .ToString()
                .Select((c, i) => i != 0 && char.IsUpper(c)
                 && !char.IsUpper(e.ToString()[i - 1]) ? " " + c : c.ToString())));

    /// <summary>
    /// Converts an enum <see cref="Type"/> to its values, for populating a ComboBox.
    /// </summary>
    public static readonly FuncValueConverter<Type, IEnumerable<Enum>> ToValues
        = new(type => type is not null && type.IsEnum
            ? Enum.GetValues(type).Cast<Enum>()
            : []);
}
