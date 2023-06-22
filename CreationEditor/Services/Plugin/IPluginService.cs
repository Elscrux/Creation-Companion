namespace CreationEditor.Services.Plugin;

public interface IPluginService {
    public IReadOnlyList<IPluginDefinition> Plugins { get; }
}