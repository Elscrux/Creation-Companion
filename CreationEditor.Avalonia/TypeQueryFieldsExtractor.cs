using Avalonia.Data.Converters;
using CreationEditor.Services.Query.Select;
using Noggog;
namespace CreationEditor.Avalonia;

public class TypeQueryFieldsExtractor() : FuncValueConverter<Type, IEnumerable<IQueryField>>(Convert) {
    private static IEnumerable<IQueryField> Convert(Type? type) {
        return type.GetAllPropertyInfos()
            .Select(field => {
                var fieldType = field.PropertyType.InheritsFrom(typeof(Nullable<>))
                    ? field.PropertyType.GetGenericArguments()[0]
                    : field.PropertyType;

                return new ReflectionQueryField(fieldType, field.Name);
            });
    }
}
