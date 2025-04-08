using System.Collections.ObjectModel;
namespace CreationEditor.Avalonia.ViewModels.Reference;

public interface IReferenceVM {
    string Name { get; }
    string Identifier { get; }
    string Type { get; }
    bool HasChildren { get; }
    ReadOnlyObservableCollection<IReferenceVM>? Children { get; }

    static int CompareName(IReferenceVM? b, IReferenceVM? a) {
        if (b is null) return -1;
        if (a is null) return 1;

        return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
    }

    static int CompareIdentifier(IReferenceVM? b, IReferenceVM? a) {
        if (b is null) return -1;
        if (a is null) return 1;

        return string.Compare(a.Identifier, b.Identifier, StringComparison.OrdinalIgnoreCase);
    }

    static int CompareType(IReferenceVM? b, IReferenceVM? a) {
        if (b is null) return -1;
        if (a is null) return 1;

        return string.Compare(a.Type, b.Type, StringComparison.OrdinalIgnoreCase);
    }

}
