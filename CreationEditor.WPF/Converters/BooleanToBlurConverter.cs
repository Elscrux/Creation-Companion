using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Win32.WinRT;
namespace CreationEditor.WPF.Converters; 

public class BooleanToBlurConverter : IValueConverter {
    private const BlurEffect Blur = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value is true ? Blur : null; 
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value != null;
    }
}