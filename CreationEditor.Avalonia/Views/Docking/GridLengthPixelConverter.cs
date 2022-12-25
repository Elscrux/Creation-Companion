using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
namespace CreationEditor.Avalonia.Views.Docking; 

public class GridLengthPixelConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value is double val ? new GridLength(val, GridUnitType.Pixel) : null;
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value is GridLength val ? val.Value : null;
    }
}
