using System.Reactive.Subjects;
namespace CreationEditor.Services.Lifecycle;

public interface ILifecycleStartup {
    /// <summary>
    /// Run before the application starts, just after the DI initialization. This runs on the main thread. 
    /// </summary>
    void PreStartup();

    /// <summary>
    /// Run when the application including UI was loaded. This runs on a separate thread.
    /// </summary>
    void PostStartupAsync(Subject<string> done, CancellationToken cancellationToken = default);

    /// <summary>
    /// Run when the application exits. This runs on the main thread.
    /// </summary>
    void Exit();

    IReadOnlyList<ILifecycleTask> LifecycleTasks { get; }
}
