using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class PlacedExtension {
    public static string? GetSelfOrBaseEditorID(this IPlacedGetter placed, ILinkCache linkCache) {
        if (placed is null) return null;

        var editorID = placed.EditorID;
        if (editorID is not null || linkCache is null) return editorID;

        switch (placed) {
            case IPlacedObjectGetter placedObjectGetter:
                if (linkCache.TryResolveIdentifier(placedObjectGetter.Base, out var obj)) {
                    return obj;
                }
                break;
            case IPlacedNpcGetter placedNpcGetter:
                if (linkCache.TryResolveIdentifier(placedNpcGetter.Base, out var npc)) {
                    return npc;
                }
                break;
            case IPlacedArrowGetter placedArrowGetter:
                if (linkCache.TryResolveIdentifier(placedArrowGetter.Projectile, out var projectile)) {
                    return projectile;
                }
                break;
            case IPlacedBarrierGetter placedBarrierGetter:
                if (linkCache.TryResolveIdentifier(placedBarrierGetter.Projectile, out var barrier)) {
                    return barrier;
                }
                break;
            case IPlacedBeamGetter placedBeamGetter:
                if (linkCache.TryResolveIdentifier(placedBeamGetter.Projectile, out var beam)) {
                    return beam;
                }
                break;
            case IPlacedConeGetter placedConeGetter:
                if (linkCache.TryResolveIdentifier(placedConeGetter.Projectile, out var cone)) {
                    return cone;
                }
                break;
            case IPlacedFlameGetter placedFlameGetter:
                if (linkCache.TryResolveIdentifier(placedFlameGetter.Projectile, out var flame)) {
                    return flame;
                }
                break;
            case IPlacedHazardGetter placedHazardGetter:
                if (linkCache.TryResolveIdentifier(placedHazardGetter.Hazard, out var hazard)) {
                    return hazard;
                }
                break;
            case IPlacedMissileGetter placedMissileGetter:
                if (linkCache.TryResolveIdentifier(placedMissileGetter.Projectile, out var missile)) {
                    return missile;
                }
                break;
            case IPlacedTrapGetter placedTrapGetter:
                if (linkCache.TryResolveIdentifier(placedTrapGetter.Projectile, out var trap)) {
                    return trap;
                }
                break;
        }

        return null;
    }
}
