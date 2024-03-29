﻿using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Utilities;
namespace CreationEditor.Avalonia.Converter;

public sealed class ExtendedFuncMultiValueConverter<TIn, TOut, TPar> : IMultiValueConverter {
    private readonly Func<IEnumerable<TIn?>, TPar?, TOut> _convert;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedFuncMultiValueConverter{TIn, TOut, TPar}"/> class.
    /// </summary>
    /// <param name="convert">The convert function.</param>
    public ExtendedFuncMultiValueConverter(Func<IEnumerable<TIn?>, TPar?, TOut> convert) {
        _convert = convert;
    }

    /// <inheritdoc/>
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
        var converted = OfTypeWithDefaultSupport(values).ToList();

        return converted.Count == values.Count && TypeUtilities.CanCast<TPar>(parameter)
            ? _convert(converted, (TPar?) parameter)
            : AvaloniaProperty.UnsetValue;

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
    }
}
