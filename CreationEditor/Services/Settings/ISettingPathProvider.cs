using Noggog;
namespace CreationEditor.Services.Settings;

public interface ISettingPathProvider {
    public FilePath Path { get; }

    public FilePath GetFullPath(ISetting setting);
}
