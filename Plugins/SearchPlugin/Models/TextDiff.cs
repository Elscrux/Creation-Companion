using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace SearchPlugin.Models;

public sealed partial class TextDiff : ReactiveObject {
    public TextDiff(string old, string @new) {
        Old = old;
        New = @new;

        IsDifferent = this.WhenAnyValue(x => x.New)
            .Select(newText => newText != Old)
            .Replay(1)
            .RefCount();
    }

    public string Old { get; }
    [Reactive] public partial string New { get; set; }

    public IObservable<bool> IsDifferent { get; }
}
