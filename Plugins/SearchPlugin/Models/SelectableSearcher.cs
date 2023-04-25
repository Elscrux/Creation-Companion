using CreationEditor.Avalonia.Models.Selectables;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace SearchPlugin.Models;

public class SelectableSearcher : ReactiveObject, IReactiveSelectable {
    public ITextSearcherDefinition Searcher { get; }
    [Reactive] public bool IsSelected { get; set; } = true;

    public SelectableSearcher(ITextSearcherDefinition searcher) {
        Searcher = searcher;
    }
}
