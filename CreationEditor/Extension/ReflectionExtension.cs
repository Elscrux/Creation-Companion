using System.Reflection;
namespace CreationEditor;

public static class ReflectionExtension {
    public static bool TryGetProperty(this object obj, string name, out object? outValue) {
        var variables = name.Split('.');
        var current = obj;
        foreach (var variable in variables) {
            var property = current.GetType().GetProperty(variable);
            if (property is null) {
                outValue = default;
                return false;
            }

            current = property.GetValue(current);
            if (current is null) {
                outValue = default;
                return false;
            }
        }

        outValue = current;
        return true;
    }

    public static bool TryGetProperty<T>(this object obj, string name, out T? outValue) {
        if (obj.TryGetProperty(name, out var value)) {
            if (value is T tValue) {
                outValue = tValue;
                return true;
            }
        }

        outValue = default;
        return false;
    }

    public static bool TrySetProperty<T>(this object obj, string name, T value) {
        var names = name.Split('.');
        var i = 0;
        var current = obj;
        PropertyInfo? property;
        while (i < names.Length - 1) {
            property = current.GetType().GetProperty(names[i]);
            if (property is null) return false;

            current = property.GetValue(obj);
            if (current is null) return false;

            i++;
        }

        property = current.GetType().GetProperty(names[^1]);
        if (property is null) return false;

        if (property.PropertyType.IsAssignableFrom(typeof(T))) {
            property.SetValue(current, value);
        } else {
            var changeType = Convert.ChangeType(value, property.PropertyType);
            property.SetValue(current, changeType);
        }
        return true;
    }
}
