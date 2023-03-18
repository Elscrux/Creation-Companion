using System.Globalization;
using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia;

public static class ConverterExtension {
    private static readonly Type PlaceholderType = typeof(string);

    public static object? Convert(this IValueConverter converter, object? value, object? parameter) {
        return converter.Convert(value, PlaceholderType, parameter, CultureInfo.InvariantCulture);
    }

    public static object? Convert(this IValueConverter converter, object? value) {
        return converter.Convert(value, null);
    }

    public static object? ConvertBack(this IValueConverter converter, object? value, object? parameter) {
        return converter.ConvertBack(value, PlaceholderType, parameter, CultureInfo.InvariantCulture);
    }

    public static object? ConvertBack(this IValueConverter converter, object? value) {
        return converter.ConvertBack(value, null);
    }
}
