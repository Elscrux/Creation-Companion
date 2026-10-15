using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Config.Options;

public sealed class DataRelativePathConfigOption(
    string name,
    string? description = null,
    DataRelativePath? defaultValue = null,
    bool isOptional = false,
    Func<DataRelativePath, ConfigOptionValidationError?>? validate = null)
    : IConfigOption<DataRelativePath> {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IsOptional { get; } = isOptional;
    public DataRelativePath DefaultValue => defaultValue ?? DataRelativePath.NullPath;
    public object DefaultObjectValue => DefaultValue;
    public Type ValueType => typeof(DataRelativePath);
    public Func<DataRelativePath, ConfigOptionValidationError?>? Validate { get; } = validate;
}
