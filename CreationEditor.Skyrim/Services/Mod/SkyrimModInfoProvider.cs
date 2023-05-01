using CreationEditor.Services.Mutagen.Mod;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Services.Mod;

public sealed class SkyrimModInfoProvider : IModInfoProvider<ISkyrimModGetter>, IModInfoProvider<IModGetter> {
    public uint GetRecordCount(ISkyrimModGetter mod) => mod.ModHeader.Stats.NumRecords;

    public uint GetRecordCount(IModGetter mod) {
        return mod is ISkyrimModGetter skyrimModGetter
            ? GetRecordCount(skyrimModGetter)
            : 0;
    }
}
