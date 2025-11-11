using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim;

public static class PackageExtension {
    extension(IPackageGetter package) {
        public IReadOnlyDictionary<sbyte, string?> GetNameFromTemplate(ILinkCache linkCache) {
            var template = package.PackageTemplate.TryResolve(linkCache);
            if (template is null) return DictionaryExt.Empty<sbyte, string?>();

            var dict = new Dictionary<sbyte, string?>();
            foreach (var (index, data) in package.Data) {
                dict[index] = data.Name ?? template.Data[index].Name;
            }

            return dict;
        }
    }
}
