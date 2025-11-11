using System.Diagnostics.CodeAnalysis;
using System.Reflection;
namespace CreationEditor;

public static class ReflectionExtension {
    extension(object obj) {
        public bool TryGetProperty(string name, [MaybeNullWhen(false)] out object outValue) {
            var variables = name.Split('.');
            var current = obj;
            foreach (var variable in variables) {
                var property = current.GetType().GetPropertyCustom(variable);
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
        public bool TryGetProperty<T>(string name, [MaybeNullWhen(false)] out T outValue) {
            if (obj.TryGetProperty(name, out var value) && value is T tValue) {
                outValue = tValue;
                return true;
            }

            outValue = default;
            return false;
        }
        public bool TrySetProperty<T>(string name, T value) {
            var names = name.Split('.');
            var i = 0;
            var current = obj;
            PropertyInfo? property;
            while (i < names.Length - 1) {
                property = current.GetType().GetPropertyCustom(names[i]);
                if (property is null) return false;

                current = property.GetValue(obj);
                if (current is null) return false;

                i++;
            }

            property = current.GetType().GetPropertyCustom(names[^1]);
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
}
