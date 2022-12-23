using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Environment;

public interface IBackgroundTaskManager {
    [Reactive] public bool ReferencesLoaded { get; set; } 
}

public class BackgroundTaskManager : IBackgroundTaskManager {
    [Reactive] public bool ReferencesLoaded { get; set; }
}
