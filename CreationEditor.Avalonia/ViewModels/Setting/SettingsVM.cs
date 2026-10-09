using CreationEditor.Services.Settings;
using DynamicData.Binding;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Setting;

public sealed partial class SettingsVM(
    ISettingProvider settingProvider,
    ISettingExporter settingExporter) : ViewModel, ISettingsVM {
    public IEnumerable<ISettingNode> RootSettings => ObservableSettings;
    private IObservableCollection<ISettingNode> ObservableSettings { get; } = new ObservableCollectionExtended<ISettingNode>(settingProvider.Settings);

    [Reactive] public partial ISettingNode? SelectedSetting { get; set; }

    [ReactiveCommand]
    private void Save() {
        foreach (var setting in RootSettings.GetAllChildren(s => s.Children, true)) {
            setting.Apply();
            settingExporter.Export(setting);
        }
    }
}
