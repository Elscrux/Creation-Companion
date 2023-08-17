namespace CreationEditor.Services.State;

public interface IStatePathProvider {
    string GetDirectoryPath();
    string GetFullPath(string state);
}
