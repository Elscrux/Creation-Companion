using Avalonia.Data.Converters;
using CreationEditor.Services.Query.Select;
using Noggog;
namespace CreationEditor.Avalonia.Converter;

public class TypeQueryFieldsExtractor() : FuncValueConverter<Type, IEnumerable<IQueryField>>(Convert) {
    public static IEnumerable<IQueryField> Convert(Type? type) {
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
}
