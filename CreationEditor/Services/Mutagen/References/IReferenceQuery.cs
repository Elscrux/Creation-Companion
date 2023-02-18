using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References;

public interface IReferenceQuery {
    public void LoadModReferences(IModGetter mod);
    public void LoadModReferences(IReadOnlyList<IModGetter> mods);
    public void LoadModReferences(ILinkCache linkCache);
    public void LoadModReferences(IGameEnvironment environment);

    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey);
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IModGetter mod);
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IReadOnlyList<IModGetter> mods);
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, ILinkCache linkCache);
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IGameEnvironment environment);
}
