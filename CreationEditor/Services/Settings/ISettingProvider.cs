namespace CreationEditor.Services.Settings;

public interface ISettingProvider {
    /// <summary>
    /// Enumerable of all settings
    /// </summary>
    IEnumerable<ISetting> Settings { get; }
}
