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
        return _convertBack is not null && TypeUtilities.CanCast<TOut>(value) && TypeUtilities.CanCast<TPar>(parameter)
            ? _convertBack((TOut?) value, (TPar?) parameter)
            : AvaloniaProperty.UnsetValue;
    }
}
