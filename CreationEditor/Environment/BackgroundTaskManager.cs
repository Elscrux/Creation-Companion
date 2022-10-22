namespace CreationEditor.Environment;

public interface IBackgroundTaskManager {
    public bool ReferencesLoaded { get; set; } 
}

public class BackgroundTaskManager : IBackgroundTaskManager {
    public bool ReferencesLoaded { get; set; }
}
