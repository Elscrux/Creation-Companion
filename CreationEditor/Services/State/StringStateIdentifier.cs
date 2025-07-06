namespace CreationEditor.Services.State;

public class StringStateIdentifier : IStateIdentifier<string> {
    public string Parse(ReadOnlySpan<char> identifier) => identifier.ToString();
    public string AsFileName(string t) => t;
}
