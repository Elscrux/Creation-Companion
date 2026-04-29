using CreationEditor.Services.Settings;
using DynamicData.Binding;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Setting;

public sealed partial class SettingsVM : ViewModel, ISettingsVM {
    private readonly ISettingExporter _settingExporter;
    public IEnumerable<ISetting> RootSettings => ObservableSettings;
    private IObservableCollection<ISetting> ObservableSettings { get; }

    [Reactive] public partial ISetting? SelectedSetting { get; set; }

    public SettingsVM(
        ISettingProvider settingProvider,
        ISettingExporter settingExporter) {
        _settingExporter = settingExporter;
        ObservableSettings = new ObservableCollectionExtended<ISetting>(settingProvider.Settings);
    }

    [ReactiveCommand]
    private void Save() {
        foreach (var setting in RootSettings.GetAllChildren(s => s.Children, true)) {
            setting.Apply();
            _settingExporter.Export(setting);
        }
    }
}
