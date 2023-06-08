namespace CreationEditor.Services.Asset; 

public interface IAssetController {
    void Move(string path, string destination);
    void Rename(string path, string destination);
    void Delete(string path);

    void Redo();
    void Undo();
}
