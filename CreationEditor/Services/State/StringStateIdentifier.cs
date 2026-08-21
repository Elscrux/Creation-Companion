namespace CreationEditor.Services.State;

public sealed class StringStateIdentifier : IStateIdentifier<string> {
    public string Parse(ReadOnlySpan<char> identifier) => identifier.ToString();
    public string AsFileName(string t) => t;
}
