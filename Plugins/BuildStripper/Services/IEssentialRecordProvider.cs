using BuildStripper.Models.FeatureFlag;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace BuildStripper.Services;

public interface IEssentialRecordProvider {
    bool IsEssentialRecord(ModKey mod, IFormLinkGetter formLink);
    Dictionary<FormKey, List<(ICellGetter Cell, ExteriorCellRetainReason RetainReason)>> EnumerateRetainedExteriorCells(ModKey mod, ILinkCache linkCache);
    IReadOnlyList<IFormLinkGetter<IRegionGetter>> GetAllowedRegions(ModKey mod, IFormLinkGetter<IWorldspaceGetter> worldspace);
}
