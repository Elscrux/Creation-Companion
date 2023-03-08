namespace CreationEditor.Services.Settings;

public interface ISettingProvider {
    public IEnumerable<ISetting> Settings { get; }
}
