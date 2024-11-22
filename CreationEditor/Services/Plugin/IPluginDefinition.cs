namespace CreationEditor.Services.Plugin;

public interface IPluginDefinition {
    string Name { get; }
    string Description { get; }
    Guid Guid { get; }
}
