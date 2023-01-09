using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.Settings;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Setting;

public sealed class SettingsVM : ViewModel, ISettingsVM {
    public IEnumerable<ISetting> RootSettings => ObservableSettings;
    private ObservableCollection<ISetting> ObservableSettings { get; }
    
    [Reactive] public ISetting? SelectedSetting { get; set; }
    
    public ReactiveCommand<Unit, Unit> Save { get; }

    public SettingsVM(
        ISettingProvider settingProvider,
        ISettingExporter settingExporter) {
        ObservableSettings = new ObservableCollection<ISetting>(settingProvider.Settings);

        Save = ReactiveCommand.Create(() => {
            foreach (var setting in GetAllSettings()) {
                settingExporter.Export(setting);
            }
        });
    }
    
    private List<ISetting> GetAllSettings() {
        var settings = new List<ISetting>();
        var settingsQueue = new Queue<ISetting>(RootSettings);

        while (settingsQueue.Count > 0) {
            var setting = settingsQueue.Dequeue();
            settings.Add(setting);

            foreach (var settingChild in setting.Children) {
                settingsQueue.Enqueue(settingChild);
            }
        }

        return settings;
    }
}
