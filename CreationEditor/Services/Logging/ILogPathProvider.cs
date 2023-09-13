namespace CreationEditor.Services.Logging;

public interface ILogPathProvider {
    string RelativeLogDirectoryPath { get; }
    string FullLogDirectoryPath { get; }

    string RelativeLogFilePath(string fileName);
    string FullLogFilePath(string fileName);
}
