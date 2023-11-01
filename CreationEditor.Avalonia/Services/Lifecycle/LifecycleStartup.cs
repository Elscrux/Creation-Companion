using System.Reactive.Subjects;
using CreationEditor.Services.Lifecycle;
using Serilog;
namespace CreationEditor.Avalonia.Services.Lifecycle;

public sealed class LifecycleStartup : ILifecycleStartup {
    private readonly ILogger _logger;

    public IReadOnlyList<ILifecycleTask> LifecycleTasks { get; }

    public LifecycleStartup(
        IReadOnlyList<ILifecycleTask> tasks,
        ILogger logger) {
        _logger = logger;
        LifecycleTasks = tasks;
    }

    public void PreStartup() {
        _logger.Here().Debug("Run {Count} Lifecycle Task(s) pre startup", LifecycleTasks.Count);

        foreach (var task in LifecycleTasks) {
            task.PreStartup();
        }
    }

    public void PostStartupAsync(Subject<string> done, CancellationToken cancellationToken) {
        _logger.Here().Debug("Run {Count} Lifecycle Task(s) post startup", LifecycleTasks.Count);

        Parallel.ForEach(LifecycleTasks, task => {
            task.PostStartupAsync(cancellationToken);
            done.OnNext(task.GetType().Name);
        });
    }

    public void Exit() {
        _logger.Here().Debug("Run {Count} Lifecycle Task(s) on Exit", LifecycleTasks.Count);

        foreach (var task in LifecycleTasks) {
            task.OnExit();
        }
    }
}
