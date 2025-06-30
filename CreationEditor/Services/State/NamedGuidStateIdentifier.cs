namespace CreationEditor.Services.State;

public record struct NamedGuid(Guid Id, string Name);

public sealed class NamedGuidStateIdentifier : IStateIdentifier<NamedGuid> {
    public const string Separator = "~";
    public NamedGuid Parse(ReadOnlySpan<char> identifier) {
        var separatorIndex = identifier.IndexOf(Separator);
        if (separatorIndex == -1) {
            return new NamedGuid(Guid.Parse(identifier), string.Empty);
        }

        var name = identifier[..separatorIndex];
        var guidSpan = identifier[(separatorIndex + Separator.Length)..];

        return new NamedGuid(Guid.Parse(guidSpan), name.ToString());
    }
    public string AsFileName(NamedGuid t) => string.IsNullOrEmpty(t.Name)
        ? t.Id.ToString()
        : t.Name + Separator + t.Id;
}
