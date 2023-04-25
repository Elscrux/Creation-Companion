using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace SearchPlugin.Models;

public class TextDiff : ReactiveObject {
    public TextDiff(string old, string @new) {
        Old = old;
        New = @new;

        IsDifferent = this.WhenAnyValue(x => x.New)
            .Select(newText => newText != Old);
    }

    public string Old { get; }
    [Reactive] public string New { get; set; }

    public IObservable<bool> IsDifferent { get; }
}
