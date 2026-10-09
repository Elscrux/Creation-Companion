namespace CreationEditor.Services.Settings;

/// <summary>
/// A node in the settings tree. Implement this when a setting only needs to be
/// displayed in the settings tree (and have its view resolved), without carrying
/// its own data model or apply behavior.
/// </summary>
public interface ISettingNode : ISetting {
    /// <summary>
    /// Parent setting class type
    /// </summary>
    Type? Parent { get; }

    /// <summary>
    /// Is automatically filled by settings provider
    /// </summary>
    List<ISettingNode> Children { get; }
}

/// <summary>
/// A setting that carries data and can be applied to the editor.
/// </summary>
public interface ISetting {
    /// <summary>
    /// Name of the setting
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Setting data
    /// </summary>
    ISettingModel Model { get; }

    /// <summary>
    /// Applies the setting to the editor
    /// </summary>
    void Apply();
}
