using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Utilities;
namespace CreationEditor.Avalonia.Converter;

public sealed class ExtendedFuncValueConverter<TIn, TOut, TPar> : IValueConverter {
    private readonly Func<TIn?, TPar?, TOut> _convert;
    private readonly Func<TOut?, TPar?, TIn>? _convertBack;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedFuncValueConverter{TIn, TOut, TPar}"/> class.
    /// </summary>
    /// <param name="convert">The convert function.</param>
    /// <param name="convertBack">The convert back function</param>
    public ExtendedFuncValueConverter(Func<TIn?, TPar?, TOut> convert, Func<TOut?, TPar?, TIn>? convertBack = null) {
        _convert = convert;
        _convertBack = convertBack;
    }

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return TypeUtilities.CanCast<TIn>(value) && TypeUtilities.CanCast<TPar>(parameter)
            ? _convert((TIn?) value, (TPar?) parameter)
            : AvaloniaProperty.UnsetValue;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return _convertBack != null && TypeUtilities.CanCast<TOut>(value) && TypeUtilities.CanCast<TPar>(parameter)
            ? _convertBack((TOut?) value, (TPar?) parameter)
            : AvaloniaProperty.UnsetValue;
    }
}
public sealed class ExtendedFuncMultiValueConverter<TIn, TOut, TPar> : IMultiValueConverter {
    private readonly Func<IEnumerable<TIn?>, TPar?, TOut> _convert;
    private readonly Func<TOut?, TPar?, TIn>? _convertBack;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedFuncMultiValueConverter{TIn, TOut, TPar}"/> class.
    /// </summary>
    /// <param name="convert">The convert function.</param>
    /// <param name="convertBack">The convert back function</param>
    public ExtendedFuncMultiValueConverter(Func<IEnumerable<TIn?>, TPar?, TOut> convert, Func<TOut?, TPar?, TIn>? convertBack = null) {
        _convert = convert;
        _convertBack = convertBack;
    }

    /// <inheritdoc/>
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
        //standard OfType skip null values, even they are valid for the Type
        static IEnumerable<TIn?> OfTypeWithDefaultSupport(IEnumerable<object?> list) {
            foreach (var obj in list) {
                if (obj is TIn result) {
                    yield return result;
                } else if (Equals(obj, default(TIn))) {
                    yield return default;
                }
            }
        }

        var converted = OfTypeWithDefaultSupport(values).ToList();

        return converted.Count == values.Count && TypeUtilities.CanCast<TPar>(parameter)
            ? _convert(converted, (TPar?) parameter)
            : AvaloniaProperty.UnsetValue;
    }
}
