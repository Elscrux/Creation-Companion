using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.Services;

public interface IBusyService : IReactiveObject {
    [Reactive] public bool IsBusy { get; set; }
}

public class BusyService : ReactiveObject, IBusyService { 
    [Reactive] public bool IsBusy { get; set; }
}
