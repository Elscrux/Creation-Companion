using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
namespace ModCleaner.Services;

public interface IEssentialRecordProvider {
    bool IsEssentialRecord(IFormLinkGetter formLink);
    bool IsInvalidExteriorCell(IFormLinkGetter<IWorldspaceGetter> worldspace, ICellGetter cell);
}
