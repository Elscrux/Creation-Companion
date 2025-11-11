using System.Globalization;
using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia;

public static class ConverterExtension {
    private static readonly Type PlaceholderType = typeof(string);

    extension(IValueConverter converter) {
        public object? Convert(object? value, object? parameter) {
            return converter.Convert(value, PlaceholderType, parameter, CultureInfo.InvariantCulture);
        }
        public object? Convert(object? value) {
            return converter.Convert(value, null);
        }
        public object? ConvertBack(object? value, object? parameter) {
            return converter.ConvertBack(value, PlaceholderType, parameter, CultureInfo.InvariantCulture);
        }
        public object? ConvertBack(object? value) {
            return converter.ConvertBack(value, null);
        }
    }
}
