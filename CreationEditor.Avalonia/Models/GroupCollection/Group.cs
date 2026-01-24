using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.GroupCollection;

public sealed partial class Group<T> : ReactiveObject, IDisposableDropoff {
    private readonly DisposableBucket _disposables = new();

    public Func<T, object?> Selector { get; }
    [Reactive] public partial bool IsGrouped { get; set; }

    public Group(Func<T, object?> selector, bool isGrouped) {
        Selector = selector;
        IsGrouped = isGrouped;
    }

    public void Dispose() => _disposables.Dispose();
    public void Add(IDisposable disposable) => _disposables.Add(disposable);
}
