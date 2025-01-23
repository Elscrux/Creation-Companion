using System.Reflection;
using CreationEditor.Services.Query.Select;
using Noggog;
namespace CreationEditor;

public static class QueryExtension {
    public static IQueryField ToQueryField(this PropertyInfo propertyInfo) {
        var fieldType = propertyInfo.PropertyType.InheritsFrom(typeof(Nullable<>))
            ? propertyInfo.PropertyType.GetGenericArguments()[0]
            : propertyInfo.PropertyType;

        return new ReflectionQueryField(fieldType, propertyInfo.Name);
    }
}
