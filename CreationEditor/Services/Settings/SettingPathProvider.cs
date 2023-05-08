using System.IO.Abstractions;
namespace CreationEditor.Services.Settings;

public sealed class SettingPathProvider : ISettingPathProvider {
    private readonly IFileSystem _fileSystem;

    public SettingPathProvider(
        IFileSystem fileSystem) {
        _fileSystem = fileSystem;
    }

    public string Path => "Settings";

    public string GetFullPath(ISetting setting) {
        return _fileSystem.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            Path,
            $"{setting.Name.Replace(" ", "-")}.json");
    }
}
