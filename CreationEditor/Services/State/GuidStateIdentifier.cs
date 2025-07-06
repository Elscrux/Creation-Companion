namespace CreationEditor.Services.State;

public sealed class GuidStateIdentifier : IStateIdentifier<Guid> {
    public Guid Parse(ReadOnlySpan<char> identifier) => Guid.Parse(identifier);
    public string AsFileName(Guid t) => t.ToString();
}
