using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CreationEditor.Services.Query;
using CreationEditor.Services.Query.Select;
namespace CreationEditor.Avalonia.Converter;

public class TypeQueryFieldsExtractor : AvaloniaObject, IValueConverter {
    private readonly IQueryFieldProvider _queryFieldProvider = new ReflectionIQueryFieldProvider();

    public static readonly StyledProperty<Func<IQueryField, bool>?> FilterProperty
        = AvaloniaProperty.Register<LevelVisibilityConverter, Func<IQueryField, bool>?>(nameof(Filter));

    public Func<IQueryField, bool>? Filter {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value switch {
            Type type => Filter is null
                ? _queryFieldProvider.FromType(type)
                : _queryFieldProvider.FromType(type).Where(Filter),
            _ => throw new InvalidOperationException()
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new InvalidOperationException();
    }
}
