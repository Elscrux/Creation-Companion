namespace CreationEditor.Avalonia.Command;

public sealed class AddElement<T> : ListCommand<T>
    where T : new() {
    public override void Execute(object? parameter) {
        List?.Add(new T());
    }
}
