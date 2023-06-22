namespace CreationEditor.Services.Asset;

public interface IModelModificationService {
    void RemapReferences(string file, Func<string, bool> shouldReplace, string newReference);
}