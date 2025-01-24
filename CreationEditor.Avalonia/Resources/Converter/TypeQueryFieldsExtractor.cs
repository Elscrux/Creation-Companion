using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CreationEditor.Services.Query.Select;
using Noggog;
namespace CreationEditor.Avalonia.Converter;

public class TypeQueryFieldsExtractor : AvaloniaObject, IValueConverter {
    public static readonly StyledProperty<Func<IQueryField, bool>?> FilterProperty
        = AvaloniaProperty.Register<LevelVisibilityConverter, Func<IQueryField, bool>?>(nameof(Filter));

    public Func<IQueryField, bool>? Filter {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public static IEnumerable<IQueryField> ConvertWithoutFilter(Type? type) {
        return type.GetAllPropertyInfos()
            .Where(field => {
                if (field.GetMethod is null) return false;

                return field.GetMethod.GetParameters().Length == 0;
            })
            .Select(field => {
                var fieldType = field.PropertyType.InheritsFrom(typeof(Nullable<>))
                    ? field.PropertyType.GetGenericArguments()[0]
                    : field.PropertyType;

                return new ReflectionQueryField(fieldType, field.Name);
            });
    }

    public static IEnumerable<IQueryField> ConvertWithFilter(Type? type, Func<IQueryField, bool> filter) {
        return ConvertWithoutFilter(type).Where(filter);
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value switch {
            Type type => Filter is null
                ? ConvertWithoutFilter(type)
                : ConvertWithFilter(type, Filter),
            _ => throw new InvalidOperationException()
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new InvalidOperationException();
    }
}
