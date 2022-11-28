using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.Services;

public interface IBusyService : IReactiveObject {
    [Reactive] public bool IsBusy { get; set; }
}