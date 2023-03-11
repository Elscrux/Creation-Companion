using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Utilities;
namespace CreationEditor.Avalonia.Converter;

public class ExtendedFuncValueConverter<TIn, TOut> : IValueConverter {
    private readonly Func<TIn?, TOut> _convert;
    private readonly Func<TOut?, TIn> _convertBack;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedFuncValueConverter{TIn, TOut}"/> class.
    /// </summary>
    /// <param name="convert">The convert function.</param>
    /// <param name="convertBack">The convert back function</param>
    public ExtendedFuncValueConverter(Func<TIn?, TOut> convert, Func<TOut?, TIn> convertBack) {
        _convert = convert;
        _convertBack = convertBack;
    }

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return TypeUtilities.CanCast<TIn>(value)
            ? _convert((TIn?) value)
            : AvaloniaProperty.UnsetValue;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return TypeUtilities.CanCast<TOut>(value)
            ? _convertBack((TOut?) value)
            : AvaloniaProperty.UnsetValue;
    }
}
