using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Extension;

public static class LocationExtension {
    public static IEnumerable<ILocationGetter> GetAllParentLocations(this ILocationGetter location, ILinkCache linkCache) {
        var currentLocation = location;
        while (!currentLocation.ParentLocation.IsNull) {
            if (currentLocation.ParentLocation.TryResolve(linkCache, out var parentLocation)) {
                yield return parentLocation;

                currentLocation = parentLocation;
            } else {
                yield break;
            }
        }
    }
}
