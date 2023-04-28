using System.IO.Abstractions;
using System.Reactive;
using Avalonia.Platform.Storage;
using CreationEditor.Avalonia.Models.Settings.Save;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Settings;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Setting.Save;

public enum SaveLocation {
    DataFolder,
    Custom
}

public sealed class SaveSettingVM : ViewModel, ISetting, ILifecycleTask {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IFileSystem _fileSystem;
    private readonly ISavePipeline _savePipeline;
    private readonly IModSaveLocationProvider _modSaveLocationProvider;

    public string Name => "Save";
    public Type? Parent => null;
    public List<ISetting> Children { get; } = new();

    public string AbsoluteCustomSaveLocation => _fileSystem.Path.IsPathRooted(Settings.DataRelativeOrAbsoluteCustomSaveLocation)
        ? Settings.DataRelativeOrAbsoluteCustomSaveLocation
        : _fileSystem.Path.Combine(_editorEnvironment.GameEnvironment.DataFolderPath, Settings.DataRelativeOrAbsoluteCustomSaveLocation);

    public ReactiveCommand<Unit, Unit> SelectCustomDirectory { get; }

    public SaveLocation[] SaveLocations { get; } = Enum.GetValues<SaveLocation>();

    public SaveSettings Settings { get; }
    public ISettingModel Model => Settings;

    private readonly IdenticalToMasterRemoveStep _identicalToMasterRemoveStep = new();

    public SaveSettingVM(
        ISettingImporter<SaveSettings> settingsImporter,
        IEditorEnvironment editorEnvironment,
        IFileSystem fileSystem,
        ISavePipeline savePipeline,
        IModSaveLocationProvider modSaveLocationProvider,
        MainWindow mainWindow) {
        _editorEnvironment = editorEnvironment;
        _fileSystem = fileSystem;
        _savePipeline = savePipeline;
        _modSaveLocationProvider = modSaveLocationProvider;
        Settings = settingsImporter.Import(this) ?? new SaveSettings();

        SelectCustomDirectory = ReactiveCommand.CreateFromTask(async () => {
            var startLocation = await mainWindow.StorageProvider
                .TryGetFolderFromPathAsync(_modSaveLocationProvider.GetSaveLocation())
                .ConfigureAwait(true);

            var folderPickerOpenOptions = new FolderPickerOpenOptions {
                Title = "Mod Save Location",
                SuggestedStartLocation = startLocation
            };

            var pickedDirectories = await mainWindow.StorageProvider
                .OpenFolderPickerAsync(folderPickerOpenOptions)
                .ConfigureAwait(true);

            var directory = pickedDirectories.Count > 0 ? pickedDirectories[0] : null;
            if (directory == null) return;

            var localPath = directory.Path.LocalPath;
            if (localPath == _editorEnvironment.GameEnvironment.DataFolderPath) {
                Settings.SaveLocation = SaveLocation.DataFolder;
            } else {
                Settings.DataRelativeOrAbsoluteCustomSaveLocation
                    = localPath.StartsWith(_editorEnvironment.GameEnvironment.DataFolderPath)
                        ? $"./{_fileSystem.Path.GetRelativePath(_editorEnvironment.GameEnvironment.DataFolderPath, localPath)}"
                        : localPath;
            }

            Apply();
        });
    }

    public void OnStartup() => Apply();

    public void OnExit() {}

    public void Apply() {
        switch (Settings.SaveLocation) {
            case SaveLocation.DataFolder:
                _modSaveLocationProvider.SaveInDataFolder();
                break;
            case SaveLocation.Custom:
                _modSaveLocationProvider.SaveInCustomDirectory(AbsoluteCustomSaveLocation);
                break;
        }

        if (Settings.RemoveIdenticalToMasterRecords) {
            _savePipeline.AddStep(_identicalToMasterRemoveStep);
        } else {
            _savePipeline.RemoveStep(_identicalToMasterRemoveStep);
        }
    }
}
