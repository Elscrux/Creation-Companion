using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
namespace ModCleaner.Services;

public interface IEssentialRecordProvider {
    IReadOnlySet<FormLinkInformation> EssentialRecords { get; }
    bool IsInvalidExteriorCell(IFormLinkGetter<IWorldspaceGetter> worldspace, ICellGetter cell);
}
