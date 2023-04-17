using CreationEditor.Avalonia.Converter;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.Converter;

public static class PlacedConverters {
    public static readonly ExtendedFuncValueConverter<IPlacedGetter, string?, ILinkCache> ToName = new((record, linkCache) => {
        if (record == null || linkCache == null) return null;

        return record.GetSelfOrBaseEditorID(linkCache);
    });
}
