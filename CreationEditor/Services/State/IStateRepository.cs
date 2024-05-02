namespace CreationEditor.Services.State;

public interface IStateRepository<T>
    where T : class {
    int Count();

    IEnumerable<StateIdentifier> LoadAllStateIdentifiers();
    IEnumerable<T> LoadAll();
    T? Load(Guid id);

    bool Save(T state, Guid id, string name = "");

    void Delete(Guid id, string name);
}
