using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Mod.Save;

/// <summary>
/// Removes compression from the mod as Mutagen doesn't support it at the moment
/// Remove when this is fixed https://github.com/Mutagen-Modding/Mutagen/issues/235
/// </summary>
public sealed class RemoveCompressionStep : ISaveStep {
    public void Execute(ILinkCache linkCache, IMod mod) {
        foreach (var record in mod.EnumerateMajorRecords().ToArray()) {
            record.IsCompressed = false;
        }
    }
}
