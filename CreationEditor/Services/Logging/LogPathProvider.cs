using System.IO.Abstractions;
namespace CreationEditor.Services.Logging;

public sealed class LogPathProvider : ILogPathProvider {
    private readonly IFileSystem _fileSystem;

    public string RelativeLogDirectoryPath => "Logs";
    public string FullLogDirectoryPath { get; }

    public LogPathProvider(
        IFileSystem fileSystem) {
        _fileSystem = fileSystem;
        FullLogDirectoryPath = fileSystem.Path.Combine(AppContext.BaseDirectory, RelativeLogDirectoryPath);
    }

    public string RelativeLogFilePath(string fileName) => _fileSystem.Path.Combine(RelativeLogDirectoryPath, fileName);
    public string FullLogFilePath(string fileName) => _fileSystem.Path.Combine(FullLogDirectoryPath, fileName);
}
