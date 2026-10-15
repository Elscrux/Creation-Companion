namespace CreationEditor.Services.Config.Options;

public sealed class FloatConfigOption(
    string name,
    string? description = null,
    float defaultValue = 0,
    float minValue = float.MinValue,
    float maxValue = float.MaxValue,
    float increment = 1,
    bool isOptional = false,
    Func<float, ConfigOptionValidationError?>? validate = null)
    : IConfigOption<float> {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IsOptional { get; } = isOptional;
    public float DefaultValue => defaultValue;
    public object DefaultObjectValue => defaultValue;
    public float MinValue { get; } = minValue;
    public float MaxValue { get; } = maxValue;
    public float Increment { get; } = increment;
    public Type ValueType => typeof(float);
    public Func<float, ConfigOptionValidationError?>? Validate { get; } = validate;
}
