using System.Diagnostics.CodeAnalysis;
using CreationEditor.Services.Environment;
using CreationEditor.Skyrim;
using ModCleaner.Services.FeatureFlag;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
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
        foreach (var featureFlag in featureFlagService.EnabledFeatureFlags) {
            foreach (var essentialRecord in featureFlag.EssentialRecords) {
                yield return essentialRecord;
            }

            foreach (var (worldspaceLink, regions) in featureFlag.AllowedRegions) {
                if (!worldspaceLink.TryResolve(editorEnvironment.LinkCache, out var worldspace)) continue;

                yield return worldspace.ToFormLinkInformation();

                foreach (var cell in worldspace.EnumerateCells()
                    .Where(c => c.Regions is not null && c.Regions.Intersect(regions).Any())) {
                    yield return cell.ToFormLinkInformation();
                }
            }
        }
    }

    public bool IsInvalidExteriorCell(IFormLinkGetter<IWorldspaceGetter> worldspace, ICellGetter cell) {
        var allowedRegions = featureFlagService.EnabledFeatureFlags.GetAllowedRegions(worldspace);

        // When there is no reference of the worldspace in any feature flag, all cells are valid
        if (allowedRegions.Count == 0) return false;

        return cell.Regions is null || !cell.Regions.Intersect(allowedRegions).Any();
    }
}
