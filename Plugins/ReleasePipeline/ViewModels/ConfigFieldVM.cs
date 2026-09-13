using System.Reactive.Linq;
using System.Reflection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// Editable wrapper around a single public property of an action's configuration object.
/// Two-way syncs the value back to the config instance via reflection.
/// </summary>
public sealed partial class ConfigFieldVM : ReactiveObject {
    private readonly PropertyInfo _property;
    private readonly object _config;

    /// <summary>Display name for the field (the property name).</summary>
    public string Name => _property.Name;

    /// <summary>Property type, used by the view to pick the right editor control.</summary>
    public Type PropertyType => _property.PropertyType;

    /// <summary>Whether the property is an enum, rendered as a ComboBox.</summary>
    public bool IsEnum => _property.PropertyType.IsEnum;

    /// <summary>Whether the property is a boolean, rendered as a CheckBox.</summary>
    public bool IsBool => _property.PropertyType == typeof(bool);

    /// <summary>Whether the property is a string, rendered as a TextBox.</summary>
    public bool IsString => _property.PropertyType == typeof(string);

    /// <summary>Whether the property is numeric, rendered as a NumericUpDown.</summary>
    public bool IsNumeric => IsNumericType(_property.PropertyType);

    /// <summary>Enum values for ComboBox rendering.</summary>
    public IReadOnlyList<object> EnumValues { get; }

    [Reactive] public partial object? Value { get; set; }

    public ConfigFieldVM(PropertyInfo property, object config) {
        _property = property;
        _config = config;
        Value = property.GetValue(config);
        EnumValues = property.PropertyType.IsEnum
            ? Enum.GetValues(property.PropertyType).Cast<object>().ToList()
            : [];

        this.WhenAnyValue(x => x.Value)
            .Skip(1)
            .Subscribe(value => {
                var converted = ConvertValue(value, property.PropertyType);
                property.SetValue(config, converted);
            });
    }

    private static object? ConvertValue(object? value, Type targetType) {
        if (value is null) return null;
        if (targetType.IsInstanceOfType(value)) return value;
        if (targetType.IsEnum) return Enum.ToObject(targetType, value);
        return Convert.ChangeType(value, targetType);
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
