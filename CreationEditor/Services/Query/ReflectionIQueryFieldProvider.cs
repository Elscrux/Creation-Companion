using System.Reflection;
using CreationEditor.Services.Query.Select;
using Noggog;
namespace CreationEditor.Services.Query;

public class ReflectionIQueryFieldProvider : IQueryFieldProvider {
    public IEnumerable<IQueryField> FromType(Type? type) {
        return type.GetAllMemberInfos<PropertyInfo>()
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
