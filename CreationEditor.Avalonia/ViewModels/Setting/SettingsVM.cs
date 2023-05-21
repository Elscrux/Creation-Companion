using System.Reactive;
using CreationEditor.Services.Settings;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Setting;

public sealed class SettingsVM : ViewModel, ISettingsVM {
    public IEnumerable<ISetting> RootSettings => ObservableSettings;
    private IObservableCollection<ISetting> ObservableSettings { get; }

    [Reactive] public ISetting? SelectedSetting { get; set; }

    public ReactiveCommand<Unit, Unit> Save { get; }

    public SettingsVM(
        ISettingProvider settingProvider,
        ISettingExporter settingExporter) {
        ObservableSettings = new ObservableCollectionExtended<ISetting>(settingProvider.Settings);

        Save = ReactiveCommand.Create(() => {
            foreach (var setting in RootSettings.GetAllChildren(s => s.Children, true)) {
                setting.Apply();
                settingExporter.Export(setting);
            }
        });
    }
}
