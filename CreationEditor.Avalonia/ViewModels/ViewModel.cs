using System.Collections;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels;

// Adapted from Noggog.WPF
public abstract class ViewModel : ReactiveObject, IActivatableViewModel, IDisposableDropoff {
    protected readonly IDisposableBucket ActivatedDisposable = new DisposableBucket();
    private readonly Lazy<CompositeDisposable> _compositeDisposable = new();
    public ViewModelActivator Activator { get; } = new();

    protected ViewModel() {
        this.WhenActivated(disposable => {
            Disposable
                .Create(() => ActivatedDisposable.Clear())
                .DisposeWith(disposable);

            WhenActivated();
        });
    }

    protected virtual void WhenActivated() {}

    protected void RaiseAndSetIfChanged<T>(ref T item, T newItem, [CallerMemberName] string? propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(item, newItem)) return;

        item = newItem;
        this.RaisePropertyChanged(propertyName);
    }

    protected void RaiseAndSetIfChanged<T>(
        ref T item,
        T newItem,
        ref bool hasBeenSet,
        bool newHasBeenSet,
        string name,
        string hasBeenSetName) {
        if (!newHasBeenSet)
            RaiseAndSetIfChanged(ref hasBeenSet, newHasBeenSet, hasBeenSetName);
        RaiseAndSetIfChanged(ref item, newItem, name);
        if (!newHasBeenSet)
            return;

        RaiseAndSetIfChanged(ref hasBeenSet, newHasBeenSet, hasBeenSetName);
    }

    protected void RaiseAndSetIfChanged<T>(
        ref T item,
        T newItem,
        BitArray hasBeenSet,
        bool newHasBeenSet,
        int index,
        string name,
        string hasBeenSetName) {
        var hasBeen = hasBeenSet[index];
        var flag = EqualityComparer<T>.Default.Equals(item, newItem);
        if (hasBeen != newHasBeenSet)
            hasBeenSet[index] = newHasBeenSet;
        if (!flag) {
            item = newItem;
            this.RaisePropertyChanged(name);
        }
        if (hasBeen == newHasBeenSet)
            return;

        this.RaisePropertyChanged(hasBeenSetName);
    }

    protected void RaiseAndSetIfChanged(
        BitArray hasBeenSet,
        bool newHasBeenSet,
        int index,
        string name) {
        if (hasBeenSet[index] == newHasBeenSet)
            return;

        hasBeenSet[index] = newHasBeenSet;
        this.RaisePropertyChanged(name);
    }

    public override string ToString() => GetType().Name;

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        ActivatedDisposable.Dispose();
        if (_compositeDisposable.IsValueCreated) {
            _compositeDisposable.Value.Dispose();
        }
    }

    public void Add(IDisposable disposable) => _compositeDisposable.Value.Add(disposable);
}
