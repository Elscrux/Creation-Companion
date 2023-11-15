using System.Reactive.Subjects;
using CreationEditor.Services.Lifecycle;
using Serilog;
namespace CreationEditor.Avalonia.Services.Lifecycle;

public sealed class LifecycleStartup(
    IReadOnlyList<ILifecycleTask> tasks,
    ILogger logger)
    : ILifecycleStartup {

    public IReadOnlyList<ILifecycleTask> LifecycleTasks { get; } = tasks;

    public void PreStartup() {
        logger.Here().Debug("Run {Count} Lifecycle Task(s) pre startup", LifecycleTasks.Count);

        foreach (var task in LifecycleTasks) {
            task.PreStartup();
        }
    }

    public void PostStartupAsync(Subject<string> done, CancellationToken cancellationToken = default) {
        logger.Here().Debug("Run {Count} Lifecycle Task(s) post startup", LifecycleTasks.Count);

        Parallel.ForEach(LifecycleTasks, task => {
            task.PostStartupAsync(cancellationToken);
            done.OnNext(task.GetType().Name);
        });
    }

    public void Exit() {
        logger.Here().Debug("Run {Count} Lifecycle Task(s) on Exit", LifecycleTasks.Count);

        foreach (var task in LifecycleTasks) {
            task.OnExit();
        }
    }
}
