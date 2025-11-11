using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class LocationExtension {
    extension(ILocationGetter location) {
        public IEnumerable<ILocationGetter> GetAllParentLocations(ILinkCache linkCache) {
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
}
