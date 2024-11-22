using System.Reactive;
using CreationEditor.Services.Settings;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Setting;

public interface ISettingsVM : IDisposableDropoff {
    IEnumerable<ISetting> RootSettings { get; }
    ISetting? SelectedSetting { get; set; }

    ReactiveCommand<Unit, Unit> Save { get; }
}
