namespace CreationEditor.Services.Query.Select;

public sealed record ReflectionQueryField(Type Type, string Name) : IQueryField {
    public object? GetValue(object? obj) {
        if (obj != null && obj.TryGetProperty(Name, out var value) && value != null) {
            return value;
        }

        return null;
    }
}
