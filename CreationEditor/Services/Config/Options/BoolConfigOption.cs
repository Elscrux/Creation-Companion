namespace CreationEditor.Services.Config.Options;

public sealed class BoolConfigOption(
    string name,
    string? description = null,
    bool defaultValue = false,
    bool isOptional = false,
    Func<bool, ConfigOptionValidationError?>? validate = null) : IConfigOption<bool> {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IsOptional { get; } = isOptional;
    public bool DefaultValue => defaultValue;
    public object DefaultObjectValue => defaultValue;
    public Type ValueType => typeof(bool);
    public Func<bool, ConfigOptionValidationError?>? Validate { get; } = validate;
}
