using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.GroupCollection;

public sealed class Group<T>(Func<T, object> selector, bool isGrouped) : ReactiveObject, IDisposableDropoff {
    private readonly DisposableBucket _disposables = new();

    public Func<T, object> Selector { get; } = selector;
    [Reactive] public bool IsGrouped { get; set; } = isGrouped;

    public void Dispose() => _disposables.Dispose();
    public void Add(IDisposable disposable) => _disposables.Add(disposable);
}
