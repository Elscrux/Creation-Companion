using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
namespace BuildStripper.Models.FeatureFlag;

public sealed record WorldspaceRegions(
    IFormLinkGetter<IWorldspaceGetter> Worldspace,
    List<IFormLinkGetter<IRegionGetter>> Regions,
    int CellViewDistanceRangeToKeepOutsidePlayableArea = 2,
    int CellLandscapeRangeToKeepOutsidePlayableArea = 4);
