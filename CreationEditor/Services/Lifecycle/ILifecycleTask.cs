namespace CreationEditor.Services.Lifecycle;

public interface ILifecycleTask {
    /// <summary>
    /// Run when the application starts
    /// </summary>
    void OnStartup();

    /// <summary>
    /// Run when the application exits
    /// </summary>
    void OnExit();
}
