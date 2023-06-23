namespace CreationEditor.Services.Settings;

public interface ISetting {
    /// <summary>
    /// Name of the setting
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Parent setting class type
    /// </summary>
    Type? Parent { get; }

    /// <summary>
    /// Is automatically filled by settings provider
    /// </summary>
    List<ISetting> Children { get; }

    /// <summary>
    /// Setting data
    /// </summary>
    ISettingModel Model { get; }

    /// <summary>
    /// Applies the setting to the editor
    /// </summary>
    void Apply();
}
