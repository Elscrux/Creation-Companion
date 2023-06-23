namespace CreationEditor.Services.Settings;

public interface ISettingExporter {
    /// <summary>
    /// Export a setting
    /// </summary>
    /// <param name="setting">Setting to export</param>
    /// <returns>True if exporting was successful, otherwise false</returns>
    bool Export(ISetting setting);
}
