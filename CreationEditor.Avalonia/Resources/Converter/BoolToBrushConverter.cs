using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
namespace CreationEditor.Avalonia.Converter;

public class BoolToBrushConverter : IValueConverter {
    public IBrush ValidBrush { get; set; } = Brushes.ForestGreen;
    public IBrush ErrorBrush { get; set; } = Brushes.IndianRed;
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is true ? ValidBrush : ErrorBrush;
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is Brush brush) return brush.Equals(ValidBrush);

        return false;
    }
}
