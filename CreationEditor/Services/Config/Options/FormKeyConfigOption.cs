using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Config.Options;

public sealed class FormKeyConfigOption(
    string name,
    IReadOnlyList<Type> recordTypes,
    string? description = null,
    FormKey? defaultValue = null,
    bool isOptional = false,
    Func<FormKey, ConfigOptionValidationError?>? validate = null) : IConfigOption<FormKey> {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IsOptional { get; } = isOptional;
    public FormKey DefaultValue { get; } = defaultValue ?? FormKey.Null;
    public object DefaultObjectValue => DefaultValue;
    public Type ValueType => typeof(FormKey);
    public IReadOnlyList<Type> RecordTypes { get; } = recordTypes;
    public Func<FormKey, ConfigOptionValidationError?>? Validate { get; } = validate;
}
