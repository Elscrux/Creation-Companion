using System.IO.Abstractions;
using Newtonsoft.Json;
using Serilog;
namespace CreationEditor.Services.Settings;

public sealed class SettingExporter : ISettingExporter {
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly ISettingPathProvider _settingPathProvider;

    public SettingExporter(
        ILogger logger,
        IFileSystem fileSystem,
        ISettingPathProvider settingPathProvider) {
        _logger = logger;
        _fileSystem = fileSystem;
        _settingPathProvider = settingPathProvider;
    }

    public bool Export(ISetting setting) {
        var filePath = _settingPathProvider.GetFullPath(setting);
        if (filePath.Directory == null) return false;

        if (!filePath.Directory.Value.Exists) filePath.Directory.Value.Create();

        _logger.Here().Information("Exporting setting {Name} to {Path}", setting.Name, filePath);
        var content = JsonConvert.SerializeObject(setting.Model, Formatting.Indented);
        _fileSystem.File.WriteAllText(filePath, content);

        return true;
    }
}
