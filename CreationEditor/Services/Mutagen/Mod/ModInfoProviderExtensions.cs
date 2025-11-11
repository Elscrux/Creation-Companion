using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
namespace CreationEditor.Services.Mutagen.Mod;

public static class ModInfoProviderExtensions {
    /// <param name="modInfoProvider">Mod info provider to get mod infos from</param>
    extension(IModInfoProvider modInfoProvider) {
        /// <summary>
        /// Build dictionary masterInfos with all masters of a single plugin recursively
        /// </summary>
        /// <param name="linkCache">Link cache to get mod infos from</param>
        /// <returns>Dictionary of ModKey to a tuple of all masters and whether all masters are valid</returns>
        public Dictionary<ModKey, (HashSet<ModKey> Masters, bool Valid)> GetMasterInfos(ILinkCache linkCache) {
            var modInfos = linkCache.ListedOrder
                .Select(modInfoProvider.GetModInfo)
                .WhereNotNull()
                .ToArray();

            return modInfoProvider.GetMasterInfos(modInfos);
        }
    }
}
