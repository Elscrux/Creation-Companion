namespace CreationEditor.Core;

public interface IMementoProvider<T> {
    T CreateMemento();
    void RestoreMemento(T memento);
}
