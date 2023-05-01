using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.GroupCollection;

public sealed class Group<T> : ReactiveObject, IDisposableDropoff {
    private readonly IDisposableDropoff _disposables = new DisposableBucket();

    public Func<T, object> Selector { get; }
    [Reactive] public bool IsGrouped { get; set; }

    public Group(Func<T, object> selector, bool isGrouped) {
        Selector = selector;
        IsGrouped = isGrouped;
    }

    public void Dispose() => _disposables.Dispose();
    public void Add(IDisposable disposable) => _disposables.Add(disposable);
}
