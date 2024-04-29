using System.Globalization;
using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Converter;

public class AppendStringConverter : IValueConverter {
    public bool Append { get; set; }
    public string? String { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (targetType == typeof(string)) return AddString(value);

        if (value is not string str) return null;

        var cleanedString = RemoveString(str);

        if (Nullable.GetUnderlyingType(targetType) is not null) targetType = targetType.GenericTypeArguments[0];
        if (targetType == typeof(int) && int.TryParse(cleanedString, out var intValue)) return intValue;
        if (targetType == typeof(float) && float.TryParse(cleanedString, out var floatValue)) return floatValue;
        if (targetType == typeof(double) && double.TryParse(cleanedString, out var doubleValue)) return doubleValue;
        if (targetType == typeof(bool) && bool.TryParse(cleanedString, out var boolValue)) return boolValue;
        if (targetType == typeof(decimal) && decimal.TryParse(cleanedString, out var decimalValue)) return decimalValue;
        if (targetType == typeof(long) && long.TryParse(cleanedString, out var longValue)) return longValue;
        if (targetType == typeof(short) && short.TryParse(cleanedString, out var shortValue)) return shortValue;
        if (targetType == typeof(byte) && byte.TryParse(cleanedString, out var byteValue)) return byteValue;
        if (targetType == typeof(sbyte) && sbyte.TryParse(cleanedString, out var sbyteValue)) return sbyteValue;
        if (targetType == typeof(uint) && uint.TryParse(cleanedString, out var uintValue)) return uintValue;
        if (targetType == typeof(ulong) && ulong.TryParse(cleanedString, out var ulongValue)) return ulongValue;
        if (targetType == typeof(ushort) && ushort.TryParse(cleanedString, out var ushortValue)) return ushortValue;
        if (targetType == typeof(Guid) && Guid.TryParse(cleanedString, out var guidValue)) return guidValue;
        if (targetType == typeof(DateTime) && DateTime.TryParse(cleanedString, culture, out var dateTimeValue)) return dateTimeValue;
        if (targetType == typeof(TimeSpan) && TimeSpan.TryParse(cleanedString, culture, out var timeSpanValue)) return timeSpanValue;

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => Convert(value, targetType, parameter, culture);

    private string AddString(object? value) {
        var format = $"{value:G9}";
        return Append ? format + String : String + format;
    }
    
    private string RemoveString(string value) {
        if (String is null) return value;

        return Append
            ? value[..^String.Length]
            : value[String.Length..];
    }
}
