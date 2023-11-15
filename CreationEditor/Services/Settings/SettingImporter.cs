using System.IO.Abstractions;
using Newtonsoft.Json;
using Serilog;
namespace CreationEditor.Services.Settings;

public sealed class SettingImporter<TSetting>(
    ILogger logger,
    IFileSystem fileSystem,
    ISettingPathProvider settingPathProvider)
    : ISettingImporter<TSetting> {

    public TSetting? Import(ISetting setting) {
        var filePath = fileSystem.FileInfo.New(settingPathProvider.GetFullPath(setting));
        if (!filePath.Exists) return default;

        logger.Here().Information("Importing setting {Name} from {Path}", setting.Name, filePath);
        var content = fileSystem.File.ReadAllText(filePath.FullName);
        return JsonConvert.DeserializeObject<TSetting>(content);
    }
}
