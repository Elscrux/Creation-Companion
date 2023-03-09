namespace CreationEditor.Avalonia.Command;

public class AddElement<T> : ListCommand<T> where T : new() {
    public override void Execute(object? parameter) {
        List?.Add(new T());
    }
}
