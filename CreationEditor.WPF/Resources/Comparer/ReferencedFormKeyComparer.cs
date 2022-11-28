using CreationEditor.WPF.Models.Record;
namespace CreationEditor.WPF.Comparer;

public class ReferencedFormKeyComparer : ReferencedComparer {
    public override int Compare(IReferencedRecord? x, IReferencedRecord? y) {
        var modKeyCompare = StringComparer.OrdinalIgnoreCase.Compare(x?.Record.FormKey.ModKey.Name, y?.Record.FormKey.ModKey.Name);
        if (modKeyCompare != 0) return modKeyCompare;
        
        return StringComparer.OrdinalIgnoreCase.Compare(x?.Record.FormKey.ID, y?.Record.FormKey.ID);
    }
}
