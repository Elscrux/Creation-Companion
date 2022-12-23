using System.Collections;
using CreationEditor.Avalonia.Models.Record;
namespace CreationEditor.Avalonia.Comparer;

public abstract class ReferencedComparer : IComparer<IReferencedRecord>, IComparer {
    public int Compare(object? x, object? y) {
        if (x is IReferencedRecord r1 && y is IReferencedRecord r2) {
            return Compare(r1, r2);
        }
        
        throw new ArgumentException($"Can't compare {x} and {y}, one of them is not IReferencedRecord");
    }
    
    public abstract int Compare(IReferencedRecord? x, IReferencedRecord? y);
}