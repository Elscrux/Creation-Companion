namespace CreationEditor.Services.Lifecycle;

public interface ILifecycle {
    /// <summary>
    /// Run when the application starts
    /// </summary>
    void Start();

    /// <summary>
    /// Run when the application exits
    /// </summary>
    void Exit();
}
