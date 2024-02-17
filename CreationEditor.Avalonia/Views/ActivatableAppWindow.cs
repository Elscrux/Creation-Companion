using System.Reactive.Disposables;
using FluentAvalonia.UI.Windowing;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views;

public class ActivatableAppWindow : AppWindow, IActivatableView, IDisposable {
    protected readonly IDisposableBucket ActivatedDisposable = new DisposableBucket();
    private readonly IDisposable _activationDisposable;

    protected ActivatableAppWindow() {
        _activationDisposable = this.WhenActivated(disposables => {
            Disposable
                .Create(() => ActivatedDisposable.Clear())
                .DisposeWith(disposables);

            WhenActivated();
        });
    }

    protected virtual void WhenActivated() {}

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposing) return;

        ActivatedDisposable.Dispose();
        _activationDisposable.Dispose();
    }
}
