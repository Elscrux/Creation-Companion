namespace CreationEditor.Avalonia.Services.Startup;

public interface ILifecycleTask {
    public void OnStartup();
    public void OnExit();
}
