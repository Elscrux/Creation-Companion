namespace CreationEditor.Services.Mutagen.References;

public interface IReferenceController<in T> {
    /// <summary>
    /// Observable that emits true when the controller is currently loading references.
    /// </summary>
    IObservable<bool> IsLoading { get; }

    /// <summary>
    /// Used to register any changes to any object and update references to it accordingly.
    /// </summary>
    /// <param name="t">Object that wasn't updated yet</param>
    /// <returns>An action that should be called with the updated object</returns>
    Action<T> RegisterUpdate(T t);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    void RegisterCreation(T t);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pair"></param>
    void RegisterDeletion(T pair);
}
