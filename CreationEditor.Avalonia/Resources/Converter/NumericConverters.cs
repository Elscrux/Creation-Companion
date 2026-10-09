using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Converter;

/// <summary>
/// Bidirectional converter between nullable long and NumericUpDown's decimal? value.
/// Unset (null) values map to BindingOperations.DoNothing so the watermark shows.
/// </summary>
public sealed class NullableLongDecimalConverter : IValueConverter {
    public static readonly NullableLongDecimalConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value switch {
            long l => (decimal) l,
            _ => BindingOperations.DoNothing,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value switch {
            decimal d => (long) d,
            null => null,
            _ => BindingOperations.DoNothing,
        };
    }
}

/// <summary>
/// Bidirectional converter between double and NumericUpDown's decimal? value.
/// </summary>
public sealed class DoubleDecimalConverter : IValueConverter {
    public static readonly DoubleDecimalConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value switch {
            double d => (decimal) d,
            _ => BindingOperations.DoNothing,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value switch {
            decimal d => (double) d,
            null => 0.0,
            _ => BindingOperations.DoNothing,
        };
    }
}
