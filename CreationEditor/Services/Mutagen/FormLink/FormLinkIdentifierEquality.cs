using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.FormLink;

public sealed class FormLinkIdentifierEqualityComparer : IEqualityComparer<IFormLinkIdentifier> {
    public static readonly FormLinkIdentifierEqualityComparer Instance = new();

    public bool Equals(IFormLinkIdentifier? x, IFormLinkIdentifier? y) {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;

        return Equals(x.FormKey, y.FormKey);
    }

    public int GetHashCode(IFormLinkIdentifier obj) {
        return HashCode.Combine(obj.FormKey);
    }
}
