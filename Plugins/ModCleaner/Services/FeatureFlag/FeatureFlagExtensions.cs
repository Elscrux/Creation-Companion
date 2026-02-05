using CreationEditor.Skyrim;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace ModCleaner.Services.FeatureFlag;

public static class FeatureFlagExtensions {
    extension(IEnumerable<Models.FeatureFlag.FeatureFlag> featureFlags) {
        public Dictionary<FormKey, List<ICellGetter>> EnumerateRetainedCells(ILinkCache linkCache) {
            var mergedFeatureFlags = featureFlags
                .SelectMany(x => x.AllowedRegions)
                .GroupBy(x => x.Worldspace);

            var retainedCells = new Dictionary<FormKey, List<ICellGetter>>();
            foreach (var group in mergedFeatureFlags) {
                if (!group.Key.TryResolve(linkCache, out var worldspace)) continue;

                var regions = group.SelectMany(y => y.Regions).ToHashSet();
                foreach (var cell in worldspace.EnumerateCells().Where(c => c.Regions is not null && c.Regions.Intersect(regions).Any())) {
                    retainedCells.GetOrAdd(worldspace.FormKey).Add(cell);
                }
            }

            return retainedCells;
        }

        public IReadOnlyList<IFormLinkGetter<IRegionGetter>> GetAllowedRegions(IFormLinkGetter<IWorldspaceGetter> worldspace) {
            return featureFlags
                .SelectMany(f => f.AllowedRegions.Find(ar => ar.Worldspace.Equals(worldspace))?.Regions ?? [])
                .ToArray();
        }
    }
}
