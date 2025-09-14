using Mutagen.Bethesda.Plugins;
namespace ModCleaner.Models;

public record FormLinkIdentifier(FormLinkInformation FormLink) : ILinkIdentifier {
    public FormLinkIdentifier(IFormLinkIdentifier formLink) : this(new FormLinkInformation(formLink.FormKey, formLink.Type)) {}
    public override string ToString() {
        return $"{FormLink.FormKey} ({FormLink.Type.Name})";
    }
}
