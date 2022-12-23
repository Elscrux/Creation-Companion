using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Services.Busy;

public interface IBusyService : IReactiveObject {
    [Reactive] public bool IsBusy { get; set; }
}