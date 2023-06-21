using System.Collections.ObjectModel;
namespace CreationEditor.Avalonia.Models.Reference;

public interface IReference {
    string Name { get; }
    string Identifier { get; }
    string Type { get; }
    bool HasChildren { get; }
    ReadOnlyObservableCollection<IReference>? Children { get; }

    static int CompareName(IReference? b, IReference? a) {
        if (b is null) return -1;
        if (a is null) return 1;

        return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
    }

    static int CompareIdentifier(IReference? b, IReference? a) {
        if (b is null) return -1;
        if (a is null) return 1;

        return string.Compare(a.Identifier, b.Identifier, StringComparison.OrdinalIgnoreCase);
    }

    static int CompareType(IReference? b, IReference? a) {
        if (b is null) return -1;
        if (a is null) return 1;

        return string.Compare(a.Type, b.Type, StringComparison.OrdinalIgnoreCase);
    }

}
