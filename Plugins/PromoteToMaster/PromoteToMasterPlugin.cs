using CreationEditor.Services.Plugin;
using Mutagen.Bethesda.Skyrim;
namespace PromoteToMaster;

public sealed class PromoteToMasterPlugin : IPlugin<ISkyrimMod, ISkyrimModGetter> {
    public string Name => "Vanilla Duplicate Cleaner";
    public string Description => "Cleans mods from vanilla duplicates.";
    public Guid Guid => new("2179f861-1934-41e7-b612-b80484542c2c");
}
