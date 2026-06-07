using CreationEditor.Services.Plugin;
namespace NifPlugin;

public sealed class NifPlugin : IPlugin {
    public string Name => "Nif Plugin";
    public string Description => "A plugin for editing NIF files.";
    public Guid Guid => new("6139a880-a7de-4672-a11e-1fc835f43c1c");
}
