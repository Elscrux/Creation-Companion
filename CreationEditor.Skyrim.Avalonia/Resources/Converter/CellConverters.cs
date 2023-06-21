using CreationEditor.Avalonia.Converter;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.Converter;

public static class CellConverters {
    public static readonly ExtendedFuncValueConverter<ICellGetter, string?, ILinkCache> ToName = new((record, linkCache) => {
        if (record is null) return null;

        var editorID = record.EditorID;
        if (editorID is not null || linkCache is null) return editorID;

        if ((record.MajorFlags & Cell.MajorFlag.Persistent) != 0) {
            if (linkCache.TryResolveSimpleContext<ICellGetter>(record.FormKey, out var cellContext)) {
                if (cellContext.Parent?.Record is IWorldspaceGetter worldspace) {
                    return $"{worldspace.EditorID} - Persistent";
                }
            }
        }

        return null;
    });
}
