namespace CreationEditor.Services.Lifecycle;

public interface ILifecycleTask {
    /// <summary>
    /// Run before the application starts, just after the DI initialization. This runs on the main thread.
    /// </summary>
    void PreStartup();

    /// <summary>
    /// Run when the application including UI was loaded. This runs on a separate thread.
    /// </summary>
    /// <param name="token"></param>
    void PostStartupAsync(CancellationToken token);

    /// <summary>
    /// Run when the application exits. This runs on the main thread.
    /// </summary>
    void OnExit();
}
