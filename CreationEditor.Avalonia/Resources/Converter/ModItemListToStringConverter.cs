using System.Globalization;
using Avalonia.Data.Converters;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Converter;

public sealed class ModItemListToStringConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is not IEnumerable<IModKeyed> modKeys) return null;

        var convert = string.Join(", ", modKeys.Select(x => x.ModKey.FileName));
        if (string.IsNullOrEmpty(convert)) return "None";

        return convert;

    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
