namespace CreationEditor.Services.Config.Options;

public sealed class LongConfigOption(
    string name,
    string? description = null,
    long defaultValue = 0,
    long minValue = long.MinValue,
    long maxValue = long.MaxValue,
    long increment = 1,
    bool isOptional = false,
    Func<long, ConfigOptionValidationError?>? validate = null)
    : IConfigOption<long> {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IsOptional { get; } = isOptional;
    public long DefaultValue => defaultValue;
    public object DefaultObjectValue => defaultValue;
    public long MinValue { get; } = minValue;
    public long MaxValue { get; } = maxValue;
    public long Increment { get; } = increment;
    public Type ValueType => typeof(long);
    public Func<long, ConfigOptionValidationError?>? Validate { get; } = validate;
}
