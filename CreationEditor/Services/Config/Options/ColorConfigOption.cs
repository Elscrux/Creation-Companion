using System.Drawing;
namespace CreationEditor.Services.Config.Options;

public sealed class ColorConfigOption(
    string name,
    string? description = null,
    Color? defaultValue = null,
    bool isOptional = false,
    Func<Color, ConfigOptionValidationError?>? validate = null) : IConfigOption<Color> {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IsOptional { get; } = isOptional;
    public Color DefaultValue { get; } = defaultValue ?? Color.White;
    public object DefaultObjectValue => DefaultValue;
    public Type ValueType => typeof(Color);
    public Func<Color, ConfigOptionValidationError?>? Validate { get; } = validate;
}
