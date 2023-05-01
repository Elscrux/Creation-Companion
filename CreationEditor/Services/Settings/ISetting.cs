﻿namespace CreationEditor.Services.Settings;

public interface ISetting {
    /// <summary>
    /// Name of the setting
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Parent setting class type
    /// </summary>
    public Type? Parent { get; }

    /// <summary>
    /// Is automatically filled by settings provider
    /// </summary>
    public List<ISetting> Children { get; }

    public ISettingModel Model { get; }

    public void Apply();
}

public interface ISettingModel {}
