namespace CreationEditor.Services.Query.Select;

public sealed record ReflectionQueryField(Type Type, string Name) : IQueryField {
    public object? GetValue(object? obj) {
        if (obj is not null && obj.TryGetProperty(Name, out var value) && value is not null) {
            return value;
        }

        return null;
    }
}
