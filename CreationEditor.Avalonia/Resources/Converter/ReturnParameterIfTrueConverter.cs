using System.Globalization;
using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Converter;

public sealed class ReturnParameterIfTrueConverter<TIn> : IValueConverter
    where TIn : IParsable<TIn> {
    public TIn? DefaultValue { get; set; }
    public object? DefaultValueConverted { get; set; }
    public IFormatProvider? ParseFormatProvider { get; set; } = null;
    public IValueConverter? Converter { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        var parsedValue = value is true ? TIn.Parse(parameter as string ?? string.Empty, ParseFormatProvider) : DefaultValue;
        if (Converter is null) return parsedValue;

        if (value is not true && DefaultValueConverted is not null) return DefaultValueConverted;

        return Converter?.Convert(parsedValue, targetType, null, CultureInfo.CurrentCulture) ?? DefaultValueConverted;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}