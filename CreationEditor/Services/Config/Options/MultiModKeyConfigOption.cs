using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Config.Options;

public sealed class MultiModKeyConfigOption(
    string name,
    string? description = null,
    ModKey[]? defaultValue = null,
    bool isOptional = false,
    Func<ModKey[], ConfigOptionValidationError?>? validate = null) : IConfigOption<ModKey[]> {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IsOptional { get; } = isOptional;
    public ModKey[] DefaultValue { get; } = defaultValue ?? [];
    public object DefaultObjectValue => DefaultValue;
    public Type ValueType => typeof(ModKey[]);
    public Func<ModKey[], ConfigOptionValidationError?>? Validate { get; } = validate;
}
