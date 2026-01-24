using System.Globalization;
using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia;

public static class ConverterExtension {
    private static readonly Type PlaceholderType = typeof(string);

    extension(IValueConverter converter) {
        public object? Convert(object? value, object? parameter) => converter.Convert(value, PlaceholderType, parameter, CultureInfo.InvariantCulture);
        public object? Convert(object? value) => converter.Convert(value, null);
        public object? ConvertBack(object? value, object? parameter) => converter.ConvertBack(value, PlaceholderType, parameter, CultureInfo.InvariantCulture);
        public object? ConvertBack(object? value) => converter.ConvertBack(value, null);
    }

    extension<TIn, TOut>(FuncValueConverter<TIn, TOut> converter) {
        public TOut? Convert(TIn? value, object? parameter) => converter.Convert(value, PlaceholderType, parameter, CultureInfo.InvariantCulture) is TOut x
            ? x
            : default;
        public TOut? Convert(TIn? value) => converter.Convert(value, null);
        public TIn? ConvertBack(TOut? value, object? parameter) => converter.ConvertBack(value, PlaceholderType, parameter, CultureInfo.InvariantCulture) is TIn x
            ? x
            : default;
        public TIn? ConvertBack(TOut? value) => converter.ConvertBack(value, null);
    }
}
