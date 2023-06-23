namespace CreationEditor.Services.Settings;

public interface ISettingPathProvider {
    /// <summary>
    /// Retrieve the full path to the file where the given setting is stored
    /// </summary>
    /// <param name="setting">Setting to get the file path for</param>
    /// <returns>Absolute path to the file where settings are stored</returns>
    string GetFullPath(ISetting setting);
}
