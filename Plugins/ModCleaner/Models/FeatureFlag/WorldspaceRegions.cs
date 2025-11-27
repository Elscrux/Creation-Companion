using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
namespace ModCleaner.Models.FeatureFlag;

public sealed record WorldspaceRegions(
    IFormLinkGetter<IWorldspaceGetter> Worldspace,
    List<IFormLinkGetter<IRegionGetter>> Regions);
