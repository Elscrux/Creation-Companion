using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public sealed class SavePipeline : ISavePipeline {
    private readonly List<ISaveStep> _steps = [];

    public SavePipeline() {
        AddStep(new IdenticalToMasterRemoveStep());
        AddStep(new DeletedNewRecordRemoveStep());
    }

    public void AddStep(ISaveStep step) {
        if (_steps.Contains(step)) return;

        _steps.Add(step);
    }

    public void RemoveStep(ISaveStep step) {
        _steps.Remove(step);
    }

    public void Execute(ILinkCache linkCache, IMod mod) {
        foreach (var step in _steps) {
            step.Execute(linkCache, mod);
        }
    }
}
