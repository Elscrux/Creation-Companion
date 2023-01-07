using System.Reactive;
using CreationEditor.Settings;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Setting; 

public interface ISettingsVM {
    public IEnumerable<ISetting> RootSettings { get; }
    public ISetting? SelectedSetting { get; set; }
    
    public ReactiveCommand<Unit, Unit> Save { get; }
}