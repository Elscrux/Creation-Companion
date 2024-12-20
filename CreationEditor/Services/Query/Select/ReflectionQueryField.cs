using DynamicData.Binding;
namespace CreationEditor.Services.Query.Select;

public sealed record ReflectionQueryField(Type Type, string Name, IQueryField? NestedField = null) : IQueryField {
    public string FullName => NestedField switch {
        ReflectionQueryField child => $"{Name}.{child.FullName}",
        _ => Name
    };

    public object? GetValue(object? obj) {
        if (obj is not null && obj.TryGetProperty(Name, out var value)) {
            if (NestedField is null) return value;

            return NestedField.GetValue(value);
        }

        return null;
    }
}

public sealed record NestedQueryField(IObservableCollection<ReflectionQueryField> Fields) : IQueryField {
    public Type Type => Fields[^1].Type;
    public string Name => string.Join(".", Fields.Select(x => x.Name));

    public object? GetValue(object? obj) {
        if (obj is not null) {
            foreach (var field in Fields) {
                obj = field.GetValue(obj);
                if (obj is null) return null;
            }

            return obj;
        }

        return null;
    }
}
