using System.IO.Abstractions;
using CreationEditor.Extension;
using Newtonsoft.Json;
using Serilog;
namespace CreationEditor.Services.Settings;

public sealed class SettingImporter<TSetting> : ISettingImporter<TSetting> {
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly ISettingPathProvider _settingPathProvider;

    public SettingImporter(
        ILogger logger,
        IFileSystem fileSystem,
        ISettingPathProvider settingPathProvider) {
        _logger = logger;
        _fileSystem = fileSystem;
        _settingPathProvider = settingPathProvider;
    }

    public TSetting? Import(ISetting setting) {
        var filePath = _settingPathProvider.GetFullPath(setting);
        if (!filePath.Exists) return default;

        _logger.Here().Information("Importing setting {Name} from {Path}", setting.Name, filePath);
        var content = _fileSystem.File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<TSetting>(content);
    }
}
