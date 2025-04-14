namespace CreationEditor.Core;

public interface IMementoProvider<T> {
    T CreateMemento();
}

public interface IMementoReceiver<T> {
    void RestoreMemento(T memento);
}
