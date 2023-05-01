using System.Reactive.Disposables;
using Avalonia.Controls;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views;

public abstract class ActivatableUserControl : UserControl, IActivatableView, IDisposable {
    protected readonly IDisposableBucket ActivatedDisposable = new DisposableBucket();
    private readonly IDisposable _activationDisposable;

    protected ActivatableUserControl() {
        _activationDisposable = this.WhenActivated(disposables => {
            Disposable
                .Create(() => ActivatedDisposable.Clear())
                .DisposeWith(disposables);

            WhenActivated();
        });
    }

    protected virtual void WhenActivated() {}

    public virtual void Dispose() {
        ActivatedDisposable.Dispose();
        _activationDisposable.Dispose();
    }
}
