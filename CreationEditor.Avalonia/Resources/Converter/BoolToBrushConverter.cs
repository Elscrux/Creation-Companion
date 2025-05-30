using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using CreationEditor.Avalonia.Constants;
namespace CreationEditor.Avalonia.Converter;

public sealed class BoolToBrushConverter : IValueConverter {
    public IBrush ValidBrush { get; set; } = StandardBrushes.ValidBrush;
    public IBrush ErrorBrush { get; set; } = StandardBrushes.InvalidBrush;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is true ? ValidBrush : ErrorBrush;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is Brush brush) return brush.Equals(ValidBrush);

        return false;
    }
}
