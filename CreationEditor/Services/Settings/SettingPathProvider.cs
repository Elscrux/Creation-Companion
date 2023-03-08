using System.IO.Abstractions;
using Noggog;
namespace CreationEditor.Services.Settings;

public sealed class SettingPathProvider : ISettingPathProvider {
    private readonly IFileSystem _fileSystem;

    public SettingPathProvider(
        IFileSystem fileSystem) {
        _fileSystem = fileSystem;
    }

    public FilePath Path => "Settings";

    public FilePath GetFullPath(ISetting setting) {
        return new FilePath(_fileSystem.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path, $"{setting.Name}.json"));
    }
}
