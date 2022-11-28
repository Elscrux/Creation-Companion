using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Loqui;
namespace CreationEditor.WPF.FormKeyPicker;

public class RecordTypeGameConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is not Type type) return null;

        return LoquiRegistration.TryGetRegister(type, out var registration)
            ? registration.ProtocolKey.Namespace
            : string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return null;
    }
}
