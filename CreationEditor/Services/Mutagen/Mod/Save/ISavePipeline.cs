using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public interface ISavePipeline {
    void AddStep(ISaveStep step);
    void RemoveStep(ISaveStep step);
    void Execute(ILinkCache linkCache, IMod mod);
}
