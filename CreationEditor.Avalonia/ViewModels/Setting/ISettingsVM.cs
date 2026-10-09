using System.Reactive;
using CreationEditor.Services.Settings;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Setting;

public interface ISettingsVM : IDisposableDropoff {
    IEnumerable<ISettingNode> RootSettings { get; }
    ISettingNode? SelectedSetting { get; set; }

    ReactiveCommand<Unit, Unit> SaveCommand { get; }
}
