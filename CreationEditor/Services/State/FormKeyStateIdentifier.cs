using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.State;

public sealed class FormKeyStateIdentifier : IStateIdentifier<FormKey> {
    public FormKey Parse(ReadOnlySpan<char> identifier) => FormKey.Factory(identifier);
    public string AsFileName(FormKey t) => t.ToFilesafeString();
}
