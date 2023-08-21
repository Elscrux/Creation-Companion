using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.GroupCollection;
using CreationEditor.Avalonia.Models.Mod;
using DynamicData.Binding;
using ReactiveUI;
using SearchPlugin.Models;
namespace SearchPlugin.ViewModels;

public interface ITextSearchVM {
    IList<SelectableSearcher> Searchers { get; }
    ObservableCollectionExtended<TextReference> References { get; }

    ReadOnlyObservableCollection<ModItem> Mods { get; }
    GroupCollection<TextReference> GroupCollection { get; }
    Group<TextReference> TypeGroup { get; }
    Group<TextReference> RecordGroup { get; }
    HierarchicalTreeDataGridSource<object> TreeStructureSource { get; }

    ReactiveCommand<Unit, Unit> SearchCommand { get; }

    string SearchText { get; set; }
    string ReplaceText { get; set; }
    bool Replace { get; set; }
    bool CaseSensitive { get; set; }
    bool IsBusy { get; set; }
}
