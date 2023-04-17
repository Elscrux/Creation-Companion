namespace CreationEditor.Services.Lifecycle;

public interface ILifecycleTask {
    public void OnStartup();
    public void OnExit();
}
