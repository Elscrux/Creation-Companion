using System.Globalization;
using Avalonia.Data.Converters;
using Loqui;
namespace CreationEditor.Avalonia.FormKeyPicker;

public class RecordTypeNameConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is not Type type) return null;

        return LoquiRegistration.TryGetRegister(type, out var register)
            ? register.ClassType.Name
            : type.Name;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return null;
    }
}
