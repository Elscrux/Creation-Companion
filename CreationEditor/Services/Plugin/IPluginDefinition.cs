namespace CreationEditor.Services.Plugin;

public interface IPluginDefinition {
    public string Name { get; }
    public string Description { get; }
    public Guid Guid { get; }
}