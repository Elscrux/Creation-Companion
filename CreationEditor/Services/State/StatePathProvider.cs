using System.IO.Abstractions;
namespace CreationEditor.Services.State;

public sealed class StatePathProvider(IFileSystem fileSystem) : IStatePathProvider {
    private const string Path = "States";

    public string GetDirectoryPath() {
        return fileSystem.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            Path);
    }

    public string GetFullPath(string state) {
        return fileSystem.Path.Combine(
            GetDirectoryPath(),
            $"{state.Replace(" ", "-")}.json");
    }
}
