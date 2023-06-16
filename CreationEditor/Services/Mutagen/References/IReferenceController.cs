namespace CreationEditor.Services.Mutagen.References;

public interface IReferenceController<in T> {
    IObservable<bool> IsLoading { get; }

    Action<T> RegisterUpdate(T record);
    void RegisterCreation(T t);
    void RegisterDeletion(T t);
}
