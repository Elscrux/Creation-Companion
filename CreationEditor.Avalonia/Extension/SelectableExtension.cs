using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.Avalonia.Models.Selectables;
using DynamicData;
using DynamicData.Binding;
using Noggog;
namespace CreationEditor.Avalonia;

public static class SelectableExtension {
    public static IObservable<Unit> SelectionChanged<T>(this ObservableCollection<T> observable)
        where T : IReactiveSelectable {
        return observable
            .ToObservableChangeSet()
            .AutoRefresh(x => x.IsSelected)
            .Unit();
    }

    public static IObservable<Unit> SelectionChanged<T>(this ReadOnlyObservableCollection<T> observable)
        where T : IReactiveSelectable {
        return observable
            .ToObservableChangeSet()
            .AutoRefresh(x => x.IsSelected)
            .Unit();
    }
}
