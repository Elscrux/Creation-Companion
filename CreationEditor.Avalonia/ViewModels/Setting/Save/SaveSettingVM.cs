using System.IO.Abstractions;
using Avalonia.Platform.Storage;
using CreationEditor.Avalonia.Models.Settings.Save;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Settings;
using Mutagen.Bethesda.Environments.DI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Setting.Save;

public sealed partial class SaveSettingVM : ViewModel, ISetting, ILifecycleTask {
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IFileSystem _fileSystem;
    private readonly ISavePipeline _savePipeline;
    private readonly IModSaveLocationProvider _modSaveLocationProvider;
    private readonly IDataSourceService _dataSourceService;
    private readonly MainWindow _mainWindow;

    public string Name => "Save";
    public Type? Parent => null;
    public List<ISetting> Children { get; } = [];

    public string FullCustomSaveLocation => _fileSystem.Path.IsPathRooted(Settings.DataRelativeOrFullCustomSaveLocation)
        ? Settings.DataRelativeOrFullCustomSaveLocation
        : _fileSystem.Path.Combine(_dataDirectoryProvider.Path, Settings.DataRelativeOrFullCustomSaveLocation);

    public SaveLocation[] SaveLocations { get; } = Enum.GetValues<SaveLocation>();

    public SaveSettings Settings { get; }
    public ISettingModel Model => Settings;
    public SingleDataSourcePickerVM DataSourcePicker { get; }

    private readonly IdenticalToMasterRemoveStep _identicalToMasterRemoveStep = new();

    public SaveSettingVM(
        ISettingImporter<SaveSettings> settingsImporter,
        IDataDirectoryProvider dataDirectoryProvider,
        IFileSystem fileSystem,
        ISavePipeline savePipeline,
        IModSaveLocationProvider modSaveLocationProvider,
        IDataSourceService dataSourceService,
        MainWindow mainWindow,
        SingleDataSourcePickerVM dataSourcePicker) {
        _dataDirectoryProvider = dataDirectoryProvider;
        _fileSystem = fileSystem;
        _savePipeline = savePipeline;
        _modSaveLocationProvider = modSaveLocationProvider;
        _dataSourceService = dataSourceService;
        _mainWindow = mainWindow;
        DataSourcePicker = dataSourcePicker;
        DataSourcePicker.Filter = ds => !ds.IsReadOnly;
        Settings = settingsImporter.Import(this) ?? new SaveSettings();

        if (Settings.SaveLocation != SaveLocation.DataFolder) {
            // Ensure that the selected data source exists, otherwise fallback to custom save location
            Settings.SaveLocation = _dataSourceService.HasDataSource(Settings.DataRelativeOrFullCustomSaveLocation)
                ? SaveLocation.DataSource
                : SaveLocation.Custom;
        }
    }

    [ReactiveCommand]
    private async Task SelectCustomDirectory() {
        var startLocation = await _mainWindow.StorageProvider.TryGetFolderFromPathAsync(_modSaveLocationProvider.GetSaveLocation())
            .ConfigureAwait(true);

        var folderPickerOpenOptions = new FolderPickerOpenOptions {
            Title = "Mod Save Location",
            SuggestedStartLocation = startLocation,
        };

        var pickedDirectories = await _mainWindow.StorageProvider.OpenFolderPickerAsync(folderPickerOpenOptions)
            .ConfigureAwait(true);

        var directory = pickedDirectories.Count > 0 ? pickedDirectories[0] : null;
        if (directory is null) return;

        var localPath = directory.Path.LocalPath;
        var dataSource = _dataSourceService.ListedOrder.FirstOrDefault(ds => string.Equals(ds.Path, localPath, StringComparison.OrdinalIgnoreCase));
        if (dataSource is not null) {
            Settings.SaveLocation = SaveLocation.DataSource;
            Settings.DataRelativeOrFullCustomSaveLocation = dataSource.Path;
        } else if (localPath == _dataDirectoryProvider.Path) {
            Settings.SaveLocation = SaveLocation.DataFolder;
        } else {
            Settings.DataRelativeOrFullCustomSaveLocation = localPath.StartsWith(_dataDirectoryProvider.Path)
                ? $"./{_fileSystem.Path.GetRelativePath(_dataDirectoryProvider.Path, localPath)}"
                : localPath;
        }

        Apply();
    }

    public void PreStartup() {}
    public void PostStartupAsync(CancellationToken token) => Apply();
    public void OnExit() {}

    public void Apply() {
        switch (Settings.SaveLocation) {
            case SaveLocation.DataFolder:
                _modSaveLocationProvider.SaveInDataFolder();
                break;
            case SaveLocation.DataSource:
                if (DataSourcePicker.SelectedDataSource is not null) {
                    Settings.DataRelativeOrFullCustomSaveLocation = DataSourcePicker.SelectedDataSource.Path;
                    _modSaveLocationProvider.SaveInCustomDirectory(DataSourcePicker.SelectedDataSource.Path);
                }
                break;
            case SaveLocation.Custom:
                _modSaveLocationProvider.SaveInCustomDirectory(FullCustomSaveLocation);
                break;
        }

        if (Settings.RemoveIdenticalToMasterRecords) {
            _savePipeline.AddStep(_identicalToMasterRemoveStep);
        } else {
            _savePipeline.RemoveStep(_identicalToMasterRemoveStep);
        }
    }
}
