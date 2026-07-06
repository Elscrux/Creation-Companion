using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace ModCleaner.Services;

public interface IEssentialRecordProvider {
    bool IsEssentialRecord(IFormLinkGetter formLink);
    bool IsInvalidExteriorCell(IFormLinkGetter<IWorldspaceGetter> worldspace, ICellGetter cell);
    Dictionary<FormKey, List<ICellGetter>> EnumerateRetainedExteriorCells(ILinkCache linkCache);
    IReadOnlyList<IFormLinkGetter<IRegionGetter>> GetAllowedRegions(IFormLinkGetter<IWorldspaceGetter> worldspace);
}
