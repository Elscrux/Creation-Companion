using CreationEditor.Services.Query.Select;
namespace CreationEditor.Services.Query;

public interface IQueryFieldProvider {
    IEnumerable<IQueryField> FromType(Type? type);
}
