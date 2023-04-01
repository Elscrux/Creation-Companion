using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Converter;

public static class DoubleConverters {
    public static readonly FuncValueConverter<double, Thickness> ToThickness
        = new(d => new Thickness(d));

    public static readonly FuncValueConverter<double, GridLength> ToGridLengthPixel
        = new(d => new GridLength(d, GridUnitType.Pixel));

    public static readonly FuncValueConverter<double, GridLength> ToGridLengthStar
        = new(d => new GridLength(d, GridUnitType.Star));

    public static readonly FuncValueConverter<double, GridLength> ToGridLengthAuto
        = new(d => new GridLength(d, GridUnitType.Auto));

    public static readonly FuncValueConverter<double, double> DivideInHalf
        = new(d => d / 2);

    public static readonly FuncMultiValueConverter<double, double> Min
        = new(doubles => doubles.Min());

    public static readonly FuncMultiValueConverter<double, double> Average
        = new(doubles => doubles.Average());

    public static readonly FuncMultiValueConverter<double, double> Max
        = new(doubles => doubles.Max());
}
