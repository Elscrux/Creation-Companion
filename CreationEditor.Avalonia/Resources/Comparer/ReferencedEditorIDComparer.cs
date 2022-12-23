using CreationEditor.Avalonia.Models.Record;
namespace CreationEditor.Avalonia.Comparer;

public class ReferencedEditorIDComparer : ReferencedComparer {
    public override int Compare(IReferencedRecord? x, IReferencedRecord? y) {
        return StringComparer.OrdinalIgnoreCase.Compare(x?.Record.EditorID, y?.Record.EditorID);
    }
}
