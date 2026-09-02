using Mutagen.Bethesda.Plugins;
namespace BuildStripper.Models;

public sealed record FormLinkIdentifier(FormLinkInformation FormLink) : ILinkIdentifier {
    public FormLinkIdentifier(IFormLinkIdentifier formLink) : this(new FormLinkInformation(formLink.FormKey, formLink.Type)) {}
    public override string ToString() {
        return $"{FormLink.FormKey} ({FormLink.Type.Name})";
    }

    public bool IsNull => FormLink.IsNull;
}
