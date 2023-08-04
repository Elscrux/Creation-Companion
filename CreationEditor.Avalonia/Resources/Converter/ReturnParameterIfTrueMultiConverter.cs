using System.Globalization;
using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Converter;

public sealed class ReturnParameterIfTrueMultiConverter<TIn> : IMultiValueConverter
    where TIn : IParsable<TIn> {
    public TIn? DefaultValue { get; set; }
    public object? DefaultValueConverted { get; set; }
    public IFormatProvider? ParseFormatProvider { get; set; } = null;
    public IValueConverter? Converter { get; set; }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
        var pass = values.All(x => x is true);
        var parsedValue = pass ? TIn.Parse(parameter as string ?? string.Empty, ParseFormatProvider) : DefaultValue;
        if (Converter is null) return parsedValue;

        if (pass is not true && DefaultValueConverted is not null) return DefaultValueConverted;

        return Converter?.Convert(parsedValue, targetType, null, CultureInfo.CurrentCulture);
    }
}
