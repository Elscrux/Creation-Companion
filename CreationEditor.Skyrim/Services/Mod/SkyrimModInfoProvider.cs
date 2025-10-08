using CreationEditor.Resources.Comparer;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Services.Mod;

public sealed class SkyrimModInfoProvider(IEditorEnvironment editorEnvironment) : IModInfoProvider<ISkyrimModGetter> {
    public static ModInfo GetModInfo(ModKey modKey, ISkyrimModHeaderGetter modHeader) {
        return new ModInfo(
            modKey,
            modHeader.Author,
            modHeader.Description,
            (modHeader.Flags & SkyrimModHeader.HeaderFlag.Localized) != 0,
            modHeader.FormVersion,
            modHeader.MasterReferences.Select(master => master.Master).ToArray());
    }

    public ModInfo GetModInfo(ModKey modKey, MutagenFrame modHeaderFrame) => GetModInfo(modKey, SkyrimModHeader.CreateFromBinary(modHeaderFrame));
    public ModInfo? GetModInfo(IModGetter mod) => mod is ISkyrimModGetter skyrimMod ? GetModInfo(skyrimMod.ModKey, skyrimMod.ModHeader) : null;
    public ModInfo GetModInfo(ISkyrimModGetter mod) => GetModInfo(mod.ModKey, mod.ModHeader);
    public ModInfo? GetModInfo(DataSourceFileLink fileLink) {
        if (!fileLink.Exists()) return null;

        var modKey = ModKey.FromFileName(fileLink.Name);
        var binaryReadParameters = new BinaryReadParameters { FileSystem = fileLink.FileSystem };
        var modPath = new ModPath(modKey, fileLink.FullPath);
        var parsingMeta = ParsingMeta.Factory(binaryReadParameters, editorEnvironment.GameEnvironment.GameRelease, modPath);
        var stream = new MutagenBinaryReadStream(fileLink.FullPath, parsingMeta);
        using var frame = new MutagenFrame(stream);
        return GetModInfo(modKey, frame);
    }

    public Dictionary<ModKey, (HashSet<ModKey> Masters, bool Valid)> GetMasterInfos(IReadOnlyList<ModInfo> modInfos) {
        var sortedMods = modInfos
            .Order(new FuncComparer<ModInfo>((a, b) => {
                // If one is a master of the other, it should come first
                if (a.Masters.Contains(b.ModKey)) return 1;
                if (b.Masters.Contains(a.ModKey)) return -1;

                //If neither is a master of the other, keep original order
                var aIndex = modInfos.IndexOf(a);
                var bIndex = modInfos.IndexOf(b);
                if (aIndex < 0 || bIndex < 0) return 0;

                return aIndex.CompareTo(bIndex);
            }))
            .ToArray();

        var masterInfos = new Dictionary<ModKey, (HashSet<ModKey> Masters, bool Valid)>();
        var modKeyIndices = sortedMods
            .Select((mod, i) => (mod.ModKey, i))
            .ToDictionary(x => x.ModKey, x => x.i);

        // Iterate in priority order
        foreach (var mod in sortedMods) {
            var masters = new HashSet<ModKey>(mod.Masters);
            var valid = true;

            // Check that all masters are valid and compile list of all recursive masters
            foreach (var master in mod.Masters) {
                if (masterInfos.TryGetValue(master, out var masterInfo) && masterInfo.Valid) {
                    masters.AddRange(masterInfo.Masters);
                    continue;
                }

                valid = false;
                break;
            }

            if (valid) {
                masters = masters.OrderBy(key => modKeyIndices[key]).ToHashSet();
            } else {
                masters.Clear();
            }

            masterInfos.Add(mod.ModKey, (masters, valid));
        }

        return masterInfos;
    }

    public uint GetRecordCount(ISkyrimModGetter mod) => mod.ModHeader.Stats.NumRecords;

    public uint GetRecordCount(IModGetter mod) {
        return mod is ISkyrimModGetter skyrimModGetter
            ? GetRecordCount(skyrimModGetter)
            : 0;
    }
}
