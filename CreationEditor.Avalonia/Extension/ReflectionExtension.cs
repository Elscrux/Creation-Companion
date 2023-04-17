using System.Reflection;
namespace CreationEditor.Avalonia;

public static class ReflectionExtension {
    public static bool TryGetProperty<T>(this object obj, string name, out T? outValue) {
        var variables = name.Split('.');
        var current = obj;
        foreach (var variable in variables) {
            var property = current.GetType().GetProperty(variable);
            if (property == null) {
                outValue = default;
                return false;
            }

            current = property.GetValue(current);
            if (current == null) {
                outValue = default;
                return false;
            }
        }

        if (current is T t) {
            outValue = t;
            return true;
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
            if (property == null) return false;

            current = property.GetValue(obj);
            if (current == null) return false;

            i++;
        }

        property = current.GetType().GetProperty(names[^1]);
        if (property == null) return false;

        if (property.PropertyType.IsAssignableFrom(typeof(T))) {
            property.SetValue(current, value);
        } else {
            var changeType = Convert.ChangeType(value, property.PropertyType);
            property.SetValue(current, changeType);
        }
        return true;
    }
}
