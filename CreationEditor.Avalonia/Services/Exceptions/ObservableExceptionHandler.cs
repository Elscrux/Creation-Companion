using System.Diagnostics;
using System.Reactive.Concurrency;
using ReactiveUI;
using Serilog;
namespace CreationEditor.Avalonia.Services.Exceptions;

public sealed class ObservableExceptionHandler(ILogger logger) : IObserver<Exception> {
    public void OnNext(Exception value) {
        if (Debugger.IsAttached) Debugger.Break();

        logger.Error("Error occured: {Message}", value.ToString());

        // RxApp.MainThreadScheduler.Schedule(() => { throw value; });
    }

    public void OnError(Exception error) {
        if (Debugger.IsAttached) Debugger.Break();

        RxApp.MainThreadScheduler.Schedule(() => throw error);
    }

    public void OnCompleted() {
        if (Debugger.IsAttached) Debugger.Break();

        RxApp.MainThreadScheduler.Schedule(() => throw new NotImplementedException());
    }
}
