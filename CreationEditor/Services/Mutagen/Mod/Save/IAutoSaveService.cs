namespace CreationEditor.Services.Mutagen.Mod.Save;

public interface IAutoSaveService {
    /// <summary>
    /// Set the settings auto saving
    /// </summary>
    /// <param name="settings">New auto save settings</param>
    void SetSettings(AutoSaveSettings settings);
}
