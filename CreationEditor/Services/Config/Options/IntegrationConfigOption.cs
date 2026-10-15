using CreationEditor.Services.Integrations;
namespace CreationEditor.Services.Config.Options;

public sealed class IntegrationConfigOption(
    string name,
    Type integrationCapabilityType,
    string? description = null,
    IntegrationConfiguration? defaultValue = null,
    bool isOptional = false,
    Func<IntegrationConfiguration, ConfigOptionValidationError?>? validate = null)
    : IConfigOption<IntegrationConfiguration> {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IsOptional { get; } = isOptional;
    public IntegrationConfiguration DefaultValue { get; } = defaultValue ?? new IntegrationConfiguration();
    public object DefaultObjectValue => DefaultValue;
    public Type ValueType => typeof(IntegrationConfiguration);
    public Type IntegrationCapabilityType { get; } = integrationCapabilityType;
    public Func<IntegrationConfiguration, ConfigOptionValidationError?>? Validate { get; } = validate;
}
