namespace CreationEditor.Resources;

public sealed class ActionDisposable(Action action) : IDisposable {
    public void Dispose() => action();
}
