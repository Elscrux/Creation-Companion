namespace CreationEditor.Services.Query;

public interface IQueryField {
    Type Type { get; }
    string Name { get; }
    object? GetValue(object? obj);
}

public sealed record QueryField(Type Type, string Name) : IQueryField {
    public object? GetValue(object? obj) {
        if (obj != null && obj.TryGetProperty(Name, out var value) && value != null) {
            return value;
        }

        return null;
    }
}
