using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Busy;

public interface IBusyService : IReactiveObject {
    bool IsBusy { get; set; }
}
