using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public class IdenticalToMasterRemoveStep : ISaveStep {
    public void Execute(ILinkCache linkCache, IMod mod) {
        foreach (var record in (mod as IModGetter).EnumerateMajorRecords()) {
            var previousOverride = linkCache.ResolveAll(record.FormKey, record.Registration.GetterType).Skip(1).FirstOrDefault();

            if (record.Equals(previousOverride)) {
                mod.Remove(record);
            }
        }
    }
}
