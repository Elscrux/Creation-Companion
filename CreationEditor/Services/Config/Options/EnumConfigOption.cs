namespace CreationEditor.Services.Config.Options;

public sealed class EnumConfigOption(
    string name,
    Type enumType,
    string? description = null,
    Enum? defaultValue = null,
    bool isOptional = false,
    Func<Enum, ConfigOptionValidationError?>? validate = null) : IConfigOption<Enum> {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IsOptional { get; } = isOptional;
    public Enum DefaultValue { get; } = defaultValue ?? (Enum) Enum.GetValues(enumType).GetValue(0)!;
    public object DefaultObjectValue => DefaultValue;
    public Type ValueType => enumType;
    public Func<Enum, ConfigOptionValidationError?>? Validate { get; } = validate;
}
