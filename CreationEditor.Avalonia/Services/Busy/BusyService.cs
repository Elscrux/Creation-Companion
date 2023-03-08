using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Services.Busy;

public sealed class BusyService : ReactiveObject, IBusyService {
    [Reactive] public bool IsBusy { get; set; }
}
