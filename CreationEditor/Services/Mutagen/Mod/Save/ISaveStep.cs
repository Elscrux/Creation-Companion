using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public interface ISaveStep {
    void Execute(ILinkCache linkCache, IMod mod);
}
