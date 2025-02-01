using CreationEditor.Avalonia.Models.Selectables;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace SearchPlugin.Models;

public sealed partial class SelectableSearcher : ReactiveObject, IReactiveSelectable {
    public required ITextSearcherDefinition Searcher { get; init; }
    [Reactive] public partial bool IsSelected { get; set; }
}
