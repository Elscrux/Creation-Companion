using CreationEditor.Avalonia.Models.Record;
using Mutagen.Bethesda.Plugins.Aspects;
namespace CreationEditor.Avalonia.Comparer;

public static class RecordComparers {
    public static readonly FuncComparer<IReferencedRecord> EditorIDComparer
        = new((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Record.EditorID, y.Record.EditorID));
    public static readonly FuncComparer<IReferencedRecord> FormKeyComparer
        = new((x, y) => {
            var modKeyCompare = StringComparer.OrdinalIgnoreCase.Compare(x.Record.FormKey.ModKey.Name, y.Record.FormKey.ModKey.Name);
            if (modKeyCompare != 0) return modKeyCompare;
        
            return StringComparer.OrdinalIgnoreCase.Compare(x.Record.FormKey.ID, y.Record.FormKey.ID);
        });
    
    public static readonly FuncSelectorComparer<IReferencedRecord, INamedRequiredGetter> NamedRequiredComparer
        = new(referencedRecord => referencedRecord.Record as INamedRequiredGetter, 
            (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name));
    
}