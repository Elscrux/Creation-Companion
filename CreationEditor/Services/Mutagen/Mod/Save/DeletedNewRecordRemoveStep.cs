using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Mod.Save;

/// <summary>
/// Fully deletes records from the mod that are created in the mod and have the deleted flag set
/// </summary>
public sealed class DeletedNewRecordRemoveStep : ISaveStep {
    public void Execute(ILinkCache linkCache, IMod mod) {
        foreach (var record in (mod as IModGetter).EnumerateMajorRecords()) {
            if (record.IsDeleted && record.FormKey.ModKey == mod.ModKey) {
                mod.Remove(record);
            }
        }
    }
}
