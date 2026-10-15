namespace CreationEditor.Services.Config.Options;

public sealed class TextConfigOption(
    string name,
    string? description = null,
    string? defaultValue = null,
    bool isOptional = false,
    Func<string, ConfigOptionValidationError?>? validate = null) : IConfigOption<string> {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IsOptional { get; } = isOptional;
    public string DefaultValue { get; } = defaultValue ?? string.Empty;
    public object DefaultObjectValue => DefaultValue;
    public Type ValueType => typeof(string);
    public Func<string, ConfigOptionValidationError?>? Validate { get; } = validate;
}
