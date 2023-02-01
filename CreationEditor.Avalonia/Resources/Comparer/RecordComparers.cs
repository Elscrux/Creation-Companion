using CreationEditor.Avalonia.Models.Record;
using Mutagen.Bethesda.Plugins.Aspects;
namespace CreationEditor.Avalonia.Comparer;

public static class RecordComparers {
    public static readonly FuncComparer<IReferencedRecord> EditorIDComparer
        = new((x, y) => {
            var xEditorID = x.Record.EditorID;
            var yEditorID = y.Record.EditorID;

            var xIsNullOrEmpty = string.IsNullOrEmpty(xEditorID);
            var yIsNullOrEmpty = string.IsNullOrEmpty(yEditorID);
            if (xIsNullOrEmpty) {
                if (yIsNullOrEmpty) return 0;
                
                return 1;
            }
            if (yIsNullOrEmpty) return -1;

            return StringComparer.OrdinalIgnoreCase.Compare(xEditorID, yEditorID);
        });
    
    public static readonly FuncComparer<IReferencedRecord> FormKeyComparer
        = new((x, y) => {
            var modKeyCompare = StringComparer.OrdinalIgnoreCase.Compare(x.Record.FormKey.ModKey.Name, y.Record.FormKey.ModKey.Name);
            if (modKeyCompare != 0) return modKeyCompare;
        
            return StringComparer.OrdinalIgnoreCase.Compare(x.Record.FormKey.ID, y.Record.FormKey.ID);
        });
    
    public static readonly FuncComparer<IReferencedRecord> ReferenceCountComparer
        = new((x, y) => x.References.Count.CompareTo(y.References.Count));
    
    public static readonly FuncSelectorComparer<IReferencedRecord, INamedRequiredGetter> NamedRequiredComparer
        = new(referencedRecord => referencedRecord.Record as INamedRequiredGetter, 
            (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name));
    
}