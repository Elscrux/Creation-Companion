using System.IO.Abstractions;
using Newtonsoft.Json;
using Serilog;
namespace CreationEditor.Services.Settings;

public sealed class JsonSettingExporter(
    ILogger logger,
    IFileSystem fileSystem,
    ISettingPathProvider settingPathProvider)
    : ISettingExporter {

    public bool Export(ISetting setting) {
        var filePath = fileSystem.FileInfo.New(settingPathProvider.GetFullPath(setting));
        if (filePath.Directory is null) return false;

        if (!filePath.Directory.Exists) filePath.Directory.Create();

        logger.Here().Information("Exporting setting {Name} to {Path}", setting.Name, filePath);
        var content = JsonConvert.SerializeObject(setting.Model, Formatting.Indented);
        fileSystem.File.WriteAllText(filePath.FullName, content);

        return true;
    }
}
