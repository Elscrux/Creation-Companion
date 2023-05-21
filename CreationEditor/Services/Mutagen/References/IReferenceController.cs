namespace CreationEditor.Services.Mutagen.References;

public interface IReferenceController<in T> {
    Action<T> RegisterUpdate(T record);
    void RegisterCreation(T t);
    void RegisterDeletion(T t);
}
