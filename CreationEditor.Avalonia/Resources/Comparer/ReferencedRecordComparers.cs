using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Mutagen.References.Record;
using Mutagen.Bethesda.Plugins.Aspects;
namespace CreationEditor.Avalonia.Comparer;

public static class ReferencedRecordComparers {
    public static readonly FuncComparer<IReferencedRecord> FormKeyComparer
        = new((x, y) => RecordComparers.FormKeyComparer.Compare(x.Record, y.Record));

    public static readonly FuncComparer<IReferencedRecord> EditorIDComparer
        = new((x, y) => RecordComparers.EditorIDComparer.Compare(x.Record, y.Record));


    public static readonly FuncSelectorComparer<IReferencedRecord, INamedRequiredGetter> NamedRequiredComparer
        = new(referencedRecord => referencedRecord.Record as INamedRequiredGetter,
            (x, y) => RecordComparers.NamedRequiredComparer.Compare(x, y));

    public static readonly FuncComparer<IReferencedRecord> RecordReferenceCountComparer
        = new((x, y) => x.RecordReferences.Count.CompareTo(y.RecordReferences.Count));

    public static readonly FuncComparer<IReferencedRecord> TypeComparer
        = new((x, y) => {
            var typeCompare = StringComparer.OrdinalIgnoreCase.Compare(x.RecordTypeName, y.RecordTypeName);
            if (typeCompare != 0) return typeCompare;

            return EditorIDComparer.Compare(x, y);
        });
}
