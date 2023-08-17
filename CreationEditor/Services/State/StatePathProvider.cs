using System.IO.Abstractions;
namespace CreationEditor.Services.State;

public sealed class StatePathProvider : IStatePathProvider {
    private readonly IFileSystem _fileSystem;

    public StatePathProvider(
        IFileSystem fileSystem) {
        _fileSystem = fileSystem;
    }

    public string Path => "States";

    public string GetDirectoryPath() {
        return _fileSystem.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            Path);
    }

    public string GetFullPath(string state) {
        return  _fileSystem.Path.Combine(
            GetDirectoryPath(),
            $"{state.Replace(" ", "-")}.json");
    }
}
