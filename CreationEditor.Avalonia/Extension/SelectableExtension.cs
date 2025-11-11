using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.Avalonia.Models.Selectables;
using DynamicData;
using DynamicData.Binding;
using Noggog;
namespace CreationEditor.Avalonia;

public static class SelectableExtension {
    extension<T>(ObservableCollection<T> observable) where T : IReactiveSelectable {
        public IObservable<Unit> SelectionChanged() {
            return observable
                .ToObservableChangeSet()
                .WhenValueChanged(x => x.IsSelected)
                .Unit();
        }
    }

    extension<T>(ReadOnlyObservableCollection<T> observable) where T : IReactiveSelectable {
        public IObservable<Unit> SelectionChanged() {
            return observable
                .ToObservableChangeSet()
                .WhenValueChanged(x => x.IsSelected)
                .Unit();
        }
    }
}
