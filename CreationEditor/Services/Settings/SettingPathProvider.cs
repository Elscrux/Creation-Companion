using System.IO.Abstractions;
namespace CreationEditor.Services.Settings;

public sealed class SettingPathProvider(IFileSystem fileSystem) : ISettingPathProvider {
    public string Path => "Settings";

    public string GetFullPath(ISetting setting) {
        return fileSystem.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            Path,
            $"{setting.Name.Replace(" ", "-")}.json");
    }
}
