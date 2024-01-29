using CreationEditor.Avalonia.Models.Selectables;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace SearchPlugin.Models;

public sealed class SelectableSearcher(ITextSearcherDefinition searcher) : ReactiveObject, IReactiveSelectable {
    public ITextSearcherDefinition Searcher { get; } = searcher;
    [Reactive] public bool IsSelected { get; set; } = true;
}
