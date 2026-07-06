using System.Diagnostics.CodeAnalysis;
using CreationEditor.Services.Environment;
using CreationEditor.Skyrim;
using ModCleaner.Services.FeatureFlag;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace ModCleaner.Services;

public sealed class EssentialRecordProvider(
    IEditorEnvironment editorEnvironment,
    IFeatureFlagService featureFlagService) : IEssentialRecordProvider {

    public bool IsEssentialRecord(IFormLinkGetter formLink) {
        if (EssentialRecords.Contains(formLink)) return true;

        return FormKeysEssentialRecords.Contains(formLink.FormKey);
    }

    [field: AllowNull, MaybeNull]
    public IReadOnlySet<FormLinkInformation> EssentialRecords => field ??= EnumerateEssentialRecords().ToHashSet();

    private IReadOnlySet<FormKey> FormKeysEssentialRecords => field ??= EssentialRecords
        .Select(fli => fli.FormKey)
        .ToHashSet();

    private IEnumerable<FormLinkInformation> EnumerateEssentialRecords() {
        foreach (var (worldspace, cells) in EnumerateRetainedExteriorCells(editorEnvironment.LinkCache)) {
            yield return new FormLinkInformation(worldspace, typeof(IWorldspaceGetter));

            foreach (var cell in cells) {
                yield return cell.ToFormLinkInformation();
            }
        }

        foreach (var featureFlag in featureFlagService.EnabledFeatureFlags) {
            foreach (var essentialRecord in featureFlag.EssentialRecords) {
                yield return essentialRecord;
            }
        }
    }

    public bool IsInvalidExteriorCell(IFormLinkGetter<IWorldspaceGetter> worldspace, ICellGetter cell) {
        var allowedRegions = GetAllowedRegions(worldspace);

        // When there is no reference of the worldspace in any feature flag, all cells are valid
        if (allowedRegions.Count == 0) return false;

        return cell.Regions is null || !cell.Regions.Intersect(allowedRegions).Any();
    }

    public Dictionary<FormKey, List<ICellGetter>> EnumerateRetainedExteriorCells(ILinkCache linkCache) {
        var mergedFeatureFlags = featureFlagService.EnabledFeatureFlags
            .SelectMany(x => x.AllowedRegions)
            .GroupBy(x => x.Worldspace);

        var retainedCells = new Dictionary<FormKey, List<ICellGetter>>();
        foreach (var group in mergedFeatureFlags) {
            if (!group.Key.TryResolve(linkCache, out var worldspace)) continue;

            var regions = group.SelectMany(y => y.Regions).ToHashSet();
            var cells = regions.Count == 0
                ? worldspace.EnumerateCells()
                : worldspace.EnumerateCells().Where(c => c.Regions is not null && c.Regions.Intersect(regions).Any());

            foreach (var cell in cells) {
                retainedCells.GetOrAdd(worldspace.FormKey).Add(cell);
            }
        }

        return retainedCells;
    }

    public IReadOnlyList<IFormLinkGetter<IRegionGetter>> GetAllowedRegions(IFormLinkGetter<IWorldspaceGetter> worldspace) {
        return featureFlagService.EnabledFeatureFlags
            .SelectMany(f => f.AllowedRegions.Find(ar => ar.Worldspace.Equals(worldspace))?.Regions ?? [])
            .ToArray();
    }
}
