using CreationEditor.Services.Plugin;
using Mutagen.Bethesda.Skyrim;
namespace PromoteToMaster;

public sealed class PromoteToMasterPlugin : IPlugin<ISkyrimMod, ISkyrimModGetter> {
    public string Name => "Promote to Master";
    public string Description => "Promotes records to a master.";
    public Guid Guid => new("5e190ab2-fd5b-44c5-bce4-34e25d8b7fcc");
}
