namespace CreationEditor.Services.Settings;

public interface ISettingPathProvider {
    public string Path { get; }

    public string GetFullPath(ISetting setting);
}
