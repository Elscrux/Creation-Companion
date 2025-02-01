using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Services.Busy;

public sealed partial class BusyService : ReactiveObject, IBusyService {
    [Reactive] public partial bool IsBusy { get; set; }
}
