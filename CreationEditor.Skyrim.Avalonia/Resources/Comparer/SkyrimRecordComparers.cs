using System;
using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Skyrim.Avalonia.Models.Record;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.Comparer;

public static class SkyrimRecordComparers {
    public static readonly FuncSelectorComparer<ReferencedPlacedRecord, IMajorRecordGetter> BaseComparer
        = new(referencedRecord => referencedRecord.Base as IMajorRecordGetter,
            (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.EditorID, y.EditorID));

    public static readonly FuncSelectorComparer<IReferencedRecord, IModeledGetter> ModeledComparer
        = new(referencedRecord => referencedRecord.Record as IModeledGetter,
            (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Model?.File.DataRelativePath, y.Model?.File.DataRelativePath));

    public static readonly FuncSelectorComparer<IReferencedRecord, ICellGetter> CellGridComparer
        = new(referencedRecord => referencedRecord.Record as ICellGetter,
            (a, b) => {
                if (a.Grid is null) {
                    if (b.Grid is null) return 0;

                    return -1;
                }
                if (b.Grid is null) return 1;

                var pointA = a.Grid.Point;
                var pointB = b.Grid.Point;

                var compareX = pointA.X.CompareTo(pointB.X);
                return compareX == 0
                    ? pointA.Y.CompareTo(pointB.Y)
                    : compareX;
            });
}
