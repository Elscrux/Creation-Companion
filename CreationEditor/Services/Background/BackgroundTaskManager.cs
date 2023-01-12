using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Background;

public interface IBackgroundTaskManager {
    [Reactive] public bool ReferencesLoaded { get; set; } 
}

public sealed class BackgroundTaskManager : IBackgroundTaskManager {
    [Reactive] public bool ReferencesLoaded { get; set; }
}
