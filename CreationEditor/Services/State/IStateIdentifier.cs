namespace CreationEditor.Services.State;

public interface IStateIdentifier<T> {
    T Parse(ReadOnlySpan<char> identifier);
    string AsFileName(T t);
}
