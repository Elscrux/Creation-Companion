namespace CreationEditor.Services.Settings;

public interface ISettingImporter<out TSetting> {
    /// <summary>
    /// Imports a setting
    /// </summary>
    /// <param name="setting">Setting to import</param>
    /// <returns>The imported setting, null if no setting could be imported</returns>
    TSetting? Import(ISetting setting);
}
