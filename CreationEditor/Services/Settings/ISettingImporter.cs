namespace CreationEditor.Services.Settings;

public interface ISettingImporter<out TSetting> {
    public TSetting? Import(ISetting setting);
}
