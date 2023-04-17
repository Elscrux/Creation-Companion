using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim;

public static class PackageExtension {
    public static IReadOnlyDictionary<sbyte, string?> GetNameFromTemplate(this IPackageGetter package, ILinkCache linkCache) {
        var template = package.PackageTemplate.TryResolve(linkCache);
        if (template == null) return DictionaryExt.Empty<sbyte, string?>();

        var dict = new Dictionary<sbyte, string?>();
        foreach (var (index, data) in package.Data) {
            dict[index] = data.Name ?? template.Data[index].Name;
        }

        return dict;
    }
}
