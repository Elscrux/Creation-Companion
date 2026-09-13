using System.Reflection;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Services.ConfigEditor;

public sealed class ConfigEditorService : IConfigEditorService {
    public IReadOnlyList<ConfigFieldVM> CreateFields(object config) {
        return config.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(IsEditable)
            .Select(property => new ConfigFieldVM(property, config))
            .ToList();
    }

    private static bool IsEditable(PropertyInfo property) {
        if (!property.CanRead || !property.CanWrite) return false;

        var type = property.PropertyType;
        return type == typeof(string)
            || type == typeof(bool)
            || type.IsEnum
            || IsNumericType(type);
    }

    private static bool IsNumericType(Type type) => type switch {
        _ when type == typeof(byte) => true,
        _ when type == typeof(sbyte) => true,
        _ when type == typeof(short) => true,
        _ when type == typeof(ushort) => true,
        _ when type == typeof(int) => true,
        _ when type == typeof(uint) => true,
        _ when type == typeof(long) => true,
        _ when type == typeof(ulong) => true,
        _ when type == typeof(float) => true,
        _ when type == typeof(double) => true,
        _ when type == typeof(decimal) => true,
        _ => false,
    };
}
