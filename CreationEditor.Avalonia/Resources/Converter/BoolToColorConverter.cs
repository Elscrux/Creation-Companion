using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
namespace CreationEditor.Avalonia.Converter;

public class BoolToColorConverter : IValueConverter {
    private ISolidColorBrush ValidBrush { get; } = Brushes.ForestGreen;
    private ISolidColorBrush ErrorBrush { get; } = Brushes.IndianRed;
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is true ? ValidBrush : ErrorBrush;
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is Brush brush) return brush.Equals(ValidBrush);

        return false;
    }
}
